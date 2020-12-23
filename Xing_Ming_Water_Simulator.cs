using System;

namespace ForestGameWaterBehaviour
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] mapSize = new int[1, 2]
            {
                {3, 3}
            };
            double gridArea = 1;
            double gravity = 9.81;
            double timeStep = 0.1;
            double mapLength = 1;
            double[,] mapRainAmountGrid = new double[mapSize[0, 0], mapSize[0, 1]];
            int iterNumber = 2;

            double[,] mapHeight = new double[mapSize[0, 0], mapSize[0, 1]];
            double[,] mapWaterGrid = new double[mapSize[0, 0], mapSize[0, 1]];
            double[,] mapWaterFlow = new double[mapSize[0, 0], mapSize[0, 1]];

            // For testing purpose
            mapWaterGrid[1, 1] = 1;
            for (int n = 0; n <= iterNumber; n++)
            {
                double[,,] mapNeighbourFlux = new double[mapSize[0, 0], mapSize[0, 1], 4];
                double[,] mapWaterIntermediateGrid = new double[mapSize[0, 0], mapSize[0, 1]];
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        mapWaterIntermediateGrid[i, j] = mapWaterGrid[i, j] + timeStep * mapRainAmountGrid[i, j];
                    }
                }

                // Left Flux
                double[,] waterContentDifferences = new double[mapSize[0, 0], mapSize[0, 1]];
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 1; j < mapSize[0, 1]; j++)
                    {
                        waterContentDifferences[i, j] = mapHeight[i, j] + mapWaterGrid[i, j] - mapHeight[i, j - 1] - mapWaterGrid[i, j - 1];
                    }
                }
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        mapNeighbourFlux[i, j, 0] = Math.Max(0, mapNeighbourFlux[i, j, 0] + timeStep * gridArea * ((gravity * waterContentDifferences[i, j]) / mapLength));
                    }
                }

                // Right Flux
                Array.Clear(waterContentDifferences, 0, waterContentDifferences.Length);
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1] - 1; j++)
                    {
                        waterContentDifferences[i, j] = mapHeight[i, j] + mapWaterGrid[i, j] - mapHeight[i, j + 1] - mapWaterGrid[i, j + 1];
                    }
                }
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        mapNeighbourFlux[i, j, 1] = Math.Max(0, mapNeighbourFlux[i, j, 1] + timeStep * gridArea * ((gravity * waterContentDifferences[i, j]) / mapLength));
                    }
                }

                // Top Flux
                Array.Clear(waterContentDifferences, 0, waterContentDifferences.Length);
                for (int i = 1; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        waterContentDifferences[i, j] = mapHeight[i, j] + mapWaterGrid[i, j] - mapHeight[i - 1, j] - mapWaterGrid[i - 1, j];
                    }
                }
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        mapNeighbourFlux[i, j, 2] = Math.Max(0, mapNeighbourFlux[i, j, 2] + timeStep * gridArea * ((gravity * waterContentDifferences[i, j]) / mapLength));
                    }
                }

                // Bottom Flux
                Array.Clear(waterContentDifferences, 0, waterContentDifferences.Length);
                for (int i = 0; i < mapSize[0, 0] - 1; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        waterContentDifferences[i, j] = mapHeight[i, j] + mapWaterGrid[i, j] - mapHeight[i + 1, j] - mapWaterGrid[i + 1, j];
                    }
                }
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        mapNeighbourFlux[i, j, 3] = Math.Max(0, mapNeighbourFlux[i, j, 3] + timeStep * gridArea * ((gravity * waterContentDifferences[i, j]) / mapLength));
                    }
                }

                // Scale back factor
                double[,] K = new double[mapSize[0, 0], mapSize[0, 1]];
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        if (Double.IsNaN( (mapWaterIntermediateGrid[i, j] * gridArea) / ((mapNeighbourFlux[i, j, 0] + mapNeighbourFlux[i, j, 1] + mapNeighbourFlux[i, j, 2] + mapNeighbourFlux[i, j, 3]) * timeStep)))
                        {
                            K[i, j] = 1;
                        }
                        else
                        {
                            K[i, j] = Math.Min(1, (mapWaterIntermediateGrid[i, j] * gridArea) / ((mapNeighbourFlux[i, j, 0] + mapNeighbourFlux[i, j, 1] + mapNeighbourFlux[i, j, 2] + mapNeighbourFlux[i, j, 3]) * timeStep));
                        }
                        for (int k = 0; k < 4; k++)
                        {
                            mapNeighbourFlux[i, j, k] = K[i, j] * mapNeighbourFlux[i, j, k];
                        }
                    }
                }

                double[,,] mapNeighbourFluxTemp = new double[mapSize[0, 0], mapSize[0, 1], 4];
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1] - 1; j++)
                    {
                        mapNeighbourFluxTemp[i, j, 0] = mapNeighbourFlux[i, j + 1, 0];
                        mapNeighbourFluxTemp[i, j + 1, 1] = mapNeighbourFlux[i, j, 1];
                    }
                }
                for (int i = 0; i < mapSize[0, 0] - 1; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        mapNeighbourFluxTemp[i, j, 2] = mapNeighbourFlux[i + 1, j, 2];
                        mapNeighbourFluxTemp[i + 1, j, 3] = mapNeighbourFlux[i, j, 3];
                    }
                }
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        mapWaterFlow[i, j] = timeStep * (mapNeighbourFluxTemp[i, j, 0] + mapNeighbourFluxTemp[i, j, 1] + mapNeighbourFluxTemp[i, j, 2] + mapNeighbourFluxTemp[i, j, 3] - mapNeighbourFlux[i, j, 0] - mapNeighbourFlux[i, j, 1] - mapNeighbourFlux[i, j, 2] - mapNeighbourFlux[i, j, 3]);
                    }
                }
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                    for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        mapWaterGrid[i, j] = mapWaterIntermediateGrid[i, j] + mapWaterFlow[i, j] / gridArea;
                    }
                }
                // Printing
                for (int i = 0; i < mapSize[0, 0]; i++)
                {
                   for (int j = 0; j < mapSize[0, 1]; j++)
                    {
                        Console.Write(string.Format("{0} ", mapWaterGrid[i, j]));
                    }
                    Console.Write(Environment.NewLine + Environment.NewLine);
                }
            }
        }
    }
}
