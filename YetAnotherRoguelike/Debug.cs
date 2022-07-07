using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace YetAnotherRoguelike
{
    public static class Debug
    {
        public static void WriteLine(object obj)
        {
            System.Diagnostics.Debug.WriteLine(obj);
        }

/*        public static Tile Fetch(int x, int y) // NOT FETCHING THE CORRECT TILES
        {
            Chunk chunkResult = ChunkAt(x, y);
            if (chunkResult == null)
            {
                return null;
            }

            // PROBLEM HERE
            // negative numbers not properly converted
            //int fixedX = x % Chunk.size + (x >= 0 ? 0 : Chunk.size), fixedY = y % Chunk.size + (y >= 0 ? 0 : Chunk.size);
            int fixedX = x >= 0 ? (x % Chunk.size) : (((x + 1) % Chunk.size) - 1) + Chunk.size;
            int fixedY = y >= 0 ? (y % Chunk.size) : (((y + 1) % Chunk.size) - 1) + Chunk.size;
            //((i + 1) % chunkSize - 1) + chunkSize
            if ((fixedX >= Chunk.size) || (fixedX < 0))
            {
                return null;
            }
            if ((fixedY >= Chunk.size) || (fixedY < 0))
            {
                return null;
            }

            return chunkResult.collection[fixedY][fixedX];
        }

        public static Tile Fetch(Vector2 position)
        {
            return Fetch((int)position.X, (int)position.Y);
        }

        public static Tile Fetch(Point position)
        {
            return Fetch(position.X, position.Y);
        }

        public static Tile.Type TypeAt(Vector2 position)
        {
            Tile result = Fetch(position);
            if (result == null)
            {
                return Tile.Type.Air;
            }
            else
            {
                return result.type;
            }
        }

        public static Chunk ChunkAt(int x, int y)
        {
            // takes in real world coordinates and returns the chunk at that position
            Vector2 chunkPosition = Chunk.ChunkPosition(x, y);
            foreach (Chunk chunk in chunks)
            {
                if (chunk.position == chunkPosition)
                {
                    return chunk;
                }
            }
            return null;
        }*/
    }
}
