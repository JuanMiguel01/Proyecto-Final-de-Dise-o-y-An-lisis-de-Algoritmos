using System;
using System.Collections.Generic;

class TShirtProblem
{
    static void Main()
    {
        // Leer n y m
        string[] input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        // Guardar probabilidades de los ingenieros por cada talla
        double[,] probabilities = new double[n, m];
        for (int i = 0; i < n; i++)
        {
            input = Console.ReadLine().Split();
            for (int j = 0; j < m; j++)
            {
                probabilities[i, j] = int.Parse(input[j]) / 1000.0;
            }
        }

        // Array para almacenar los resultados de DP
        double[,] dp = new double[m, n + 1]; // dp para cada tamaño de camiseta

        // Array para la suma acumulada de probabilidades
        double[] sumDp = new double[m];

        // Inicializamos DP para cada talla
        for (int size = 0; size < m; size++)
        {
            dp[size, 0] = 1.0; // Caso base: nadie tiene esa camiseta

            // Calculamos las probabilidades acumuladas de que los ingenieros usen esa talla
            for (int i = 1; i <= n; i++)
            {
                dp[size, i] = dp[size, i - 1] * (1 - probabilities[i - 1, size]);
            }

            // Sumamos las probabilidades acumuladas
            sumDp[size] = 1.0 - dp[size, n];
        }

        // Lista para priorizar las diferencias de probabilidades entre tallas
        List<double> values = new List<double>();
        for (int size = 0; size < m; size++)
        {
            values.Add(sumDp[size]);
        }

        // Sumamos las N diferencias de probabilidades más grandes
        double result = 0.0;
        for (int i = 0; i < n; i++)
        {
            // Encontramos la mayor diferencia y la agregamos al resultado
            int maxIndex = -1;
            double maxValue = -1;
            for (int j = 0; j < m; j++)
            {
                if (sumDp[j] > maxValue)
                {
                    maxValue = sumDp[j];
                    maxIndex = j;
                }
            }

            // Actualizamos el resultado
            result += maxValue;

            // Actualizamos la DP de la talla que acabamos de usar
            UpdateDp(probabilities, dp, sumDp, maxIndex, n);
        }

        // Imprimimos el resultado con la precisión requerida
        Console.WriteLine($"{result:F12}");
    }

    // Función para actualizar la DP de una talla después de asignarla a un ingeniero
    static void UpdateDp(double[,] probabilities, double[,] dp, double[] sumDp, int size, int n)
{
    // Copiamos los valores de DP anteriores
    double[] tmp = new double[n + 1];
    
    // Copiamos manualmente la fila 'size' de la matriz dp
    for (int i = 0; i <= n; i++)
    {
        tmp[i] = dp[size, i];
    }

    // Recalculamos la DP para la talla `size`
    dp[size, 0] = 0;
    for (int i = 1; i <= n; i++)
    {
        dp[size, i] = tmp[i - 1] * probabilities[i - 1, size] + dp[size, i - 1] * (1 - probabilities[i - 1, size]);
    }

    // Actualizamos la diferencia de probabilidades
    sumDp[size] -= dp[size, n];
}

}
