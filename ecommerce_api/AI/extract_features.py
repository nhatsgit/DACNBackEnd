import os
import numpy as np
import pandas as pd
from PIL import Image
from torchvision import models, transforms
import torch
from sqlalchemy import create_engine
from sqlalchemy.sql import text

# Chuỗi kết nối cơ sở dữ liệu
connection_string = "mssql+pyodbc://DESKTOP-NEDCV2T\\SQLEXPRESS/EComerceDB?driver=ODBC+Driver+17+for+SQL+Server&trusted_connection=yes"
engine = create_engine(connection_string)

# Đường dẫn gốc đến thư mục wwwroot
root_path = "C:/Users/HP/Desktop/ecommerce_api/ecommerce_api/wwwroot"

# Hàm tải ảnh từ đường dẫn và trích xuất đặc trưng
def extract_feature_vector(image_path):
    # Tải ảnh từ thư mục wwwroot
    try:
        img = Image.open(image_path).convert("RGB")
    except Exception as e:
        print(f"Lỗi khi mở ảnh {image_path}: {e}")
        return None
    
    # Định dạng lại ảnh cho mô hình ResNet
    preprocess = transforms.Compose([
        transforms.Resize(256),
        transforms.CenterCrop(224),
        transforms.ToTensor(),
        transforms.Normalize(mean=[0.485, 0.456, 0.406], std=[0.229, 0.224, 0.225]),
    ])
    img_tensor = preprocess(img).unsqueeze(0)  # Thêm chiều batch

    # Sử dụng mô hình ResNet-50 đã huấn luyện sẵn để trích xuất đặc trưng
    model = models.resnet50(pretrained=True)
    model.eval()  # Chuyển mô hình về chế độ đánh giá (evaluation)
    
    with torch.no_grad():
        feature_vector = model(img_tensor)  # Trích xuất vector đặc trưng
        feature_vector = feature_vector.flatten().numpy()  # Chuyển sang mảng NumPy

    return feature_vector

# Kết nối và lấy danh sách sản phẩm từ cơ sở dữ liệu
def get_products_from_db():
    try:
        with engine.connect() as connection:
            print("Kết nối cơ sở dữ liệu thành công!")
            query = text("SELECT ProductId, AnhDaiDien FROM Products WHERE AnhDaiDien IS NOT NULL")
            result = connection.execute(query)
            products = [row for row in result]
            return products
    except Exception as e:
        print(f"Lỗi khi truy vấn cơ sở dữ liệu: {e}")
        return []

# Lưu vector đặc trưng vào bảng ProductFeature
import json

def save_feature_to_db(product_id, feature_vector):
    try:
        with engine.connect() as connection:
            # Chuyển feature_vector thành chuỗi JSON
            feature_vector_json = json.dumps(feature_vector.tolist())  # Chuyển mảng NumPy thành list và chuyển thành JSON
            
            query = text("""
                INSERT INTO ProductFeature (ProductId, FeatureVector)
                VALUES (:product_id, :feature_vector)
            """)
            
            # Truyền tham số dưới dạng từ điển
            connection.execute(query, {'product_id': product_id, 'feature_vector': feature_vector_json})
            
            # Commit giao dịch để lưu dữ liệu vào DB
            connection.commit()
            print(f"Đã lưu vector đặc trưng cho sản phẩm {product_id}")
    except Exception as e:
        print(f"Lỗi khi lưu vector đặc trưng cho sản phẩm {product_id}: {e}")


# Main function: Trích xuất và lưu vector cho tất cả sản phẩm
def main():
    products = get_products_from_db()  # Lấy danh sách sản phẩm từ DB

    for product in products:
        product_id, image_path = product
        full_image_path = os.path.join(root_path, image_path.lstrip("/"))  # Tạo đường dẫn đầy đủ đến ảnh

        # Kiểm tra xem ảnh có tồn tại không
        if not os.path.exists(full_image_path):
            print(f"Ảnh không tồn tại: {full_image_path}")
            continue

        # Trích xuất vector đặc trưng
        feature_vector = extract_feature_vector(full_image_path)

        if feature_vector is not None:
            # Lưu vector vào cơ sở dữ liệu

            save_feature_to_db(product_id, feature_vector)

if __name__ == "__main__":
    main()
