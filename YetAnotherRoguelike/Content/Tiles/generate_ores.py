from PIL import Image
import os

current_directory = os.path.dirname(os.path.realpath(__file__))

"""
Coal_ore,
Bauxite,
Hematite,
Sphalerite, // Tin
Calamine, // Zinc
Galena,
Cinnabar, // Mercury
Argentite, // Silver
Bismuth,
"""

ore_types = [
    "Coal_ore",
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
    # target_path = os.path.join(current_directory, str(ore).lower())

    # if not os.path.exists(target_path):
    #     os.mkdir(target_path)

    ore_image = Image.open(f"Ores/Ore_content/{ore}.png")
    stone_image = Image.open(f"stone.png")

    index = 0
    for y in range(0, 5):
        for x in range(0, 5):
            index += 1
            crop = (x * pixel_size, y * pixel_size, (1 + x) * pixel_size, (1 + y) * pixel_size)
            stone_image.paste(ore_image, crop, ore_image)
    stone_image.save(f"{str(ore).lower()}.png")
