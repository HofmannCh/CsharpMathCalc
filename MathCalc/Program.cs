namespace MathCalc
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var exs = new[] {
                //"1 + 1",
                //"2 * 3",
                //"2 + 2 * 4",
                //"(2 + 2) * 4",
                //"3 * (2 + 2)",
                //"1234",
                //"(1234)",
                //"(3 + (1 + 1)) * 2",
                "19 / (84 * (16 / 16) + (51 * 99 * (4 ** 4)) / 62 - 13) + 39)",
                //"19 / (84 * (16 / 16) + (51 * 99 * (4 ** 4)) / 62 - 13) + 39)",
                //"(51 * 99 * (4 ** 4)) / 62 - 13",
                //"19 / (84 * (16 / 16) + (51 * 99 * (4 ** 4)) / 62 - 13)",
                //"84 * (16 / 16) + (51 * 99 * (4 ** 4)) / 62 - 13",
                //"(84 * (16 / 16) + (51 * 99 * (4 ** 4)) / 62 - 13)",
            };

            var calc = new Calculatur();

            foreach (var expression in exs)
            {
                try
                {
                    Console.WriteLine("Expression: " + expression);
                    var res = calc.EvaluateExpression(expression);
                    Console.WriteLine($"Result: " + res);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Result: " + ex.GetType().FullName + ": " + ex.Message);
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
