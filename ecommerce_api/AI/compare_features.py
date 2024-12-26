import sys
import numpy as np
import json
from sklearn.metrics.pairwise import cosine_similarity
from sqlalchemy import create_engine
from sqlalchemy.sql import text
from PIL import Image
from torchvision import models, transforms
import torch

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
def load_feature_from_db(product_id):
    connection_string = "mssql+pyodbc://DESKTOP-NEDCV2T\\SQLEXPRESS/EComerceDB?driver=ODBC+Driver+17+for+SQL+Server&trusted_connection=yes"
    engine = create_engine(connection_string)
    
    try:
        with engine.connect() as connection:
            query = text("""
                SELECT FeatureVector FROM ProductFeature WHERE ProductId = :product_id
            """)
            result = connection.execute(query, {'product_id': product_id}).fetchone()
            if result:
                return np.array(json.loads(result[0]))
            else:
                return None
    except Exception as e:
        print(f"Lỗi khi tải vector đặc trưng từ cơ sở dữ liệu: {e}")
        return None

def compare_features(query_feature_vector):
    # Lấy danh sách tất cả các productId và vector đặc trưng từ cơ sở dữ liệu
    connection_string = "mssql+pyodbc://DESKTOP-NEDCV2T\\SQLEXPRESS/EComerceDB?driver=ODBC+Driver+17+for+SQL+Server&trusted_connection=yes"
    engine = create_engine(connection_string)
    
    try:
        with engine.connect() as connection:
            query = text("SELECT ProductId, FeatureVector FROM ProductFeature")
            results = connection.execute(query).fetchall()

            similarities = []
            for result in results:
                product_id = result[0]
                feature_vector = np.array(json.loads(result[1]))  # Chuyển FeatureVector thành mảng NumPy

                # Tính similarity (cosine similarity)
                similarity = cosine_similarity([query_feature_vector], [feature_vector])[0][0]
                
                # Lọc chỉ những sản phẩm có similarity > 0.5
                if similarity > 0.7:
                    similarities.append((product_id, similarity))

            # Sắp xếp danh sách theo độ tương đồng giảm dần
            similarities.sort(key=lambda x: x[1], reverse=True)
            return similarities
    except Exception as e:
        print(f"Lỗi khi so sánh vector đặc trưng: {e}")
        return []

if __name__ == "__main__":
    image_path = sys.argv[1]

    # Trích xuất vector đặc trưng từ ảnh
    feature_vector = extract_feature_vector(image_path)

    if feature_vector is not None:
        # So sánh với các vector trong cơ sở dữ liệu
        result = compare_features(feature_vector)

        # In kết quả: danh sách productID với độ tương đồng
        for product_id, similarity in result:
            print(f"Product ID: {product_id}, Similarity: {similarity}")
