from PIL import Image
name = "stone"
image = Image.open(f"{name}_tileset.png")

pixel_size = 16

index = 0
for y in range(0, 4):
    for x in range(0, 4):
        index += 1
        crop = (x*pixel_size, y*pixel_size, (1+x)*pixel_size, (1+y)*pixel_size)
        print(crop)
        new = image.crop(crop)
        new.save(f'{name}/{index}.png')
