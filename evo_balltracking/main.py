import numpy
from cvzone.ColorModule import ColorFinder
import cvzone
import cv2 as cv
import socket

capture = cv.VideoCapture(0)
# width
capture.set(3, 1280)
# height
capture.set(4, 720)

isTrue, frame = capture.read()
h, w, _ = frame.shape

# Calculate screen center
centerX = w // 2  # منتصف العرض
centerY = h // 2  # منتصف الارتفاع

# color
myColorFinder = ColorFinder(False)
hsvVals ={'hmin': 46, 'smin': 68, 'vmin': 66, 'hmax': 87, 'smax': 184, 'vmax': 255}

# establish connection
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAdressPort = ("127.0.0.1", 5054)

while True:
    isTrue, frame = capture.read()
    frame = cv.flip(frame, 1)
    imgcolor, mask = myColorFinder.update(frame, hsvVals) 
    imgContour, contours = cvzone.findContours(frame, mask)
    if contours:
        # Calculate relative position
        relativeX = contours[0]['center'][0] - centerX
        relativeY = centerY - contours[0]['center'][1]
        
        # Calculate Z-axis (based on area)
        area = contours[0]['area']
        z = 1 / (area / 10000)  # Normalize the area to a Z value

        # Send data
        data = relativeX, relativeY, round(z, 2)  # Round Z for better readability
        print(data)
        sock.sendto(str.encode(str(data)), serverAdressPort)

    # stack
    imagStack = cvzone.stackImages([frame, imgcolor, mask, imgContour], cols=2, scale=0.5)
    cv.imshow("image", imagStack)
    imgContour = cv.resize(imgContour, (0, 0), None, 0.3, 0.3)
    cv.imshow("imageContour", imgContour)

    # Exit condition: Press 'q' to quit
    if cv.waitKey(1) & 0xFF == ord('q'):
        print("Exiting...")
        break

# Release the resources
capture.release()
cv.waitKey(1)
