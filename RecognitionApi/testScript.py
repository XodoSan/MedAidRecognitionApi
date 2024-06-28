import os
from ultralytics import YOLO

model = YOLO('best.pt')

directory = 'C:/Users/Andrew/Desktop/InputPictures/'

files = os.listdir(directory)

for item in files:
	model.predict(directory + item, save=True, imgsz=320, hide_conf=True, hide_labels=True, project='/Users/n.n.anikin/MedAidRecognitionApi
')