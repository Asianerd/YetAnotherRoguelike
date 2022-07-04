from PIL import Image
image = Image.open('default.png')

pixel_size = 4

index = 0
for y in range(0, 3):
    for x in range(0, 3):
        index += 1
        crop = (x*pixel_size, y*pixel_size, (1+x)*pixel_size, (1+y)*pixel_size)
        print(crop)
        new = image.crop(crop)
        new.save(f'default{index}.png')
