from PIL import Image
import math

data = []
size = 2048
hsize = size / 2
origin = [size / 2, size / 2]

for y in range(size):
    for x in range(size):
        c = round((1 - (math.sqrt(
            ((origin[0] - x) ** 2) +
            ((origin[1] - y) ** 2)
        ) / hsize)) * 255)
        data.append((c, c, c, c))

final = Image.open(r'D:\GithubRepos\YetAnotherRoguelike\YetAnotherRoguelike\Content\Light\radial.png')
final.putdata(data)
final.save(r'D:\GithubRepos\YetAnotherRoguelike\YetAnotherRoguelike\Content\Light\radial.png')

final.show()
