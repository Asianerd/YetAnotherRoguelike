from PIL import Image
import os

current_directory = os.path.dirname(os.path.realpath(__file__))

ore_types = [
    "Bauxite",
    "Hematite",
    "Sphalerite",
    "Calamine",
    "Galena",
    "Cinnabar",
    "Argentite",
    "Bismuth"
]

pixel_size = 16

for ore in ore_types:
    target_path = os.path.join(current_directory, str(ore).lower())

    if not os.path.exists(target_path):
        os.mkdir(target_path)

    ore_image = Image.open(f"Ores/Ore_content/{ore}.png")
    stone_image = Image.open(f"stone_tileset.png")

    index = 0
    for y in range(0, 4):
        for x in range(0, 4):
            index += 1
            crop = (x * pixel_size, y * pixel_size, (1 + x) * pixel_size, (1 + y) * pixel_size)
            new_image = stone_image.crop(crop)
            new_image.paste(ore_image, (0, 0), ore_image)
            new_image.save(f"{str(ore).lower()}/{index}.png")
