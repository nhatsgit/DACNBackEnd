import sys
import json
import numpy as np
from PIL import Image
from torchvision import models, transforms
import torch
from sqlalchemy import create_engine
from sqlalchemy.sql import text

def extract_feature_vector(image_path):
    img = Image.open(image_path).convert("RGB")
    
    preprocess = transforms.Compose([
        transforms.Resize(256),
        transforms.CenterCrop(224),
        transforms.ToTensor(),
        transforms.Normalize(mean=[0.485, 0.456, 0.406], std=[0.229, 0.224, 0.225]),
    ])
    img_tensor = preprocess(img).unsqueeze(0)

    model = models.resnet50(pretrained=True)
    model.eval()
    
    with torch.no_grad():
        feature_vector = model(img_tensor)
        feature_vector = feature_vector.flatten().numpy()

    return feature_vector

def save_feature_to_db(product_id, feature_vector):
    connection_string = "mssql+pyodbc://DESKTOP-NEDCV2T\\SQLEXPRESS/EComerceDB?driver=ODBC+Driver+17+for+SQL+Server&trusted_connection=yes"
    engine = create_engine(connection_string)

    try:
        with engine.connect() as connection:
            feature_vector_json = json.dumps(feature_vector.tolist())
            query = text("""
                INSERT INTO ProductFeature (ProductId, FeatureVector)
                VALUES (:product_id, :feature_vector)
            """)
            connection.execute(query, {'product_id': product_id, 'feature_vector': feature_vector_json})
            connection.commit()
            print(f"Đã lưu vector đặc trưng cho sản phẩm {product_id}")
            sys.exit(0)  # Thành công
    except Exception as e:
        print(f"Lỗi khi lưu vector đặc trưng cho sản phẩm {product_id}: {e}")
        sys.exit(2)  # Lỗi

if __name__ == "__main__":
    product_id = int(sys.argv[1])
    image_path = sys.argv[2]
    
    feature_vector = extract_feature_vector(image_path)
    
    if feature_vector is not None:
        save_feature_to_db(product_id, feature_vector)
