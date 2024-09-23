using System;
using System.Collections.Generic;
using System.Linq;

class BattleshipGA
{
    private int n; // Tamaño del tablero
    private int[] rowTally, colTally;
    private List<int> fleet; // Flota de barcos
    private Random rand = new Random();
    private const int PopulationSize = 100;
    private const int MaxGenerations = 10000;
    private const double MutationRate = 0.05;
    private const double CrossoverRate = 0.7;

    public BattleshipGA(int[] rowTally, int[] colTally, List<int> fleet)
    {
        this.n = rowTally.Length;
        this.rowTally = rowTally;
        this.colTally = colTally;
        this.fleet = fleet;
    }

    // Representar un individuo como una lista de barcos (x, y, longitud, horizontal)
    private List<(int x, int y, int length, bool horizontal)> GenerateIndividual()
    {
        var individual = new List<(int x, int y, int length, bool horizontal)>();
        foreach (int shipLength in fleet)
        {
            bool horizontal = rand.NextDouble() > 0.5;
            int x = rand.Next(n - (horizontal ? 0 : shipLength));
            int y = rand.Next(n - (horizontal ? shipLength : 0));
            individual.Add((x, y, shipLength, horizontal));
        }
        return individual;
    }

    // Generar una población inicial
    private List<List<(int x, int y, int length, bool horizontal)>> GeneratePopulation()
    {
        var population = new List<List<(int x, int y, int length, bool horizontal)>>(); // Usar nombres en la tupla
        for (int i = 0; i < PopulationSize; i++)
        {
            population.Add(GenerateIndividual());
        }
        return population;
    }

    // Función de fitness: penalizar soluciones que no cumplen con las restricciones
    private int Fitness(List<(int x, int y, int length, bool horizontal)> individual)
    {
        int[,] grid = new int[n, n];
        int penalty = 0;

        // Colocar barcos en el tablero
        foreach (var (x, y, length, horizontal) in individual)
        {
            for (int i = 0; i < length; i++)
            {
                int nx = x + (horizontal ? 0 : i);
                int ny = y + (horizontal ? i : 0);
                grid[nx, ny] = 1;
            }
        }

        // Penalizar si no cumple con las restricciones de filas y columnas
        for (int i = 0; i < n; i++)
        {
            int rowCount = 0, colCount = 0;
            for (int j = 0; j < n; j++)
            {
                if (grid[i, j] == 1) rowCount++;
                if (grid[j, i] == 1) colCount++;
            }
            penalty += Math.Abs(rowCount - rowTally[i]) + Math.Abs(colCount - colTally[i]);
        }

        // Penalizar si hay barcos adyacentes (ortogonal o diagonalmente)
        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                if (grid[x, y] == 1)
                {
                    foreach (var (dx, dy) in new[] { (-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1) })
                    {
                        int adjX = x + dx, adjY = y + dy;
                        if (adjX >= 0 && adjX < n && adjY >= 0 && adjY < n && grid[adjX, adjY] == 1)
                        {
                            penalty += 100; // Penalización fuerte por barcos adyacentes
                        }
                    }
                }
            }
        }

        return penalty;
    }

    // Seleccionar un individuo mediante selección por torneo
    private List<(int x, int y, int length, bool horizontal)> TournamentSelection(List<List<(int x, int y, int length, bool horizontal)>> population)
    {
        int tournamentSize = 5;
        var tournament = new List<List<(int x, int y, int length, bool horizontal)>>();

        for (int i = 0; i < tournamentSize; i++)
        {
            tournament.Add(population[rand.Next(PopulationSize)]);
        }

        return tournament.OrderBy(ind => Fitness(ind)).First();
    }

    // Cruzar dos individuos
    private List<(int x, int y, int length, bool horizontal)> Crossover(List<(int x, int y, int length, bool horizontal)> parent1, List<(int x, int y, int length, bool horizontal)> parent2)
    {
        if (rand.NextDouble() > CrossoverRate)
            return new List<(int x, int y, int length, bool horizontal)>(parent1);

        int crossoverPoint = rand.Next(fleet.Count);
        var child = new List<(int x, int y, int length, bool horizontal)>();
        for (int i = 0; i < fleet.Count; i++)
        {
            child.Add(i < crossoverPoint ? parent1[i] : parent2[i]);
        }
        return child;
    }

    // Mutar un individuo
    private void Mutate(List<(int x, int y, int length, bool horizontal)> individual)
    {
        if (rand.NextDouble() < MutationRate)
        {
            int index = rand.Next(individual.Count);
            bool newOrientation = rand.NextDouble() > 0.5;
            int newX = rand.Next(n - (newOrientation ? 0 : individual[index].length));
            int newY = rand.Next(n - (newOrientation ? individual[index].length : 0));
            individual[index] = (newX, newY, individual[index].length, newOrientation);
        }
    }

    // Ejecutar el algoritmo genético
    public List<(int x, int y, int length, bool horizontal)> Solve()
    {
        var population = GeneratePopulation();
        for (int generation = 0; generation < MaxGenerations; generation++)
        {
            var newPopulation = new List<List<(int x, int y, int length, bool horizontal)>>();

            for (int i = 0; i < PopulationSize; i++)
            {
                var parent1 = TournamentSelection(population);
                var parent2 = TournamentSelection(population);
                var child = Crossover(parent1, parent2);
                Mutate(child);
                newPopulation.Add(child);
            }

            population = newPopulation;

            // Buscar la mejor solución en la población actual
            var best = population.OrderBy(ind => Fitness(ind)).First();
            if (Fitness(best) == 0)
            {
                return best; // Solución encontrada
            }
        }

        // Si no se encuentra una solución perfecta, retornar la mejor
        return population.OrderBy(ind => Fitness(ind)).First();
    }

    // Imprimir el tablero
    public void PrintSolution(List<(int x, int y, int length, bool horizontal)> solution)
    {
        int[,] grid = new int[n, n];
        foreach (var (x, y, length, horizontal) in solution)
        {
            for (int i = 0; i < length; i++)
            {
                int nx = x + (horizontal ? 0 : i);
                int ny = y + (horizontal ? i : 0);
                grid[nx, ny] = 1;
            }
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write(grid[i, j] == 1 ? "B " : "~ ");
            }
            Console.WriteLine();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        int[] rowTally = { 2, 2, 2, 2 };
        int[] colTally = { 2, 2, 2, 2 };
        List<int> fleet = new List<int> { 2 ,2};

        BattleshipGA ga = new BattleshipGA(rowTally, colTally, fleet);
        var solution = ga.Solve();

        if (solution != null)
        {
            Console.WriteLine("Solución encontrada:");
            ga.PrintSolution(solution);
        }
        else
        {
            Console.WriteLine("No se pudo encontrar una solución.");
        }
    }
}
