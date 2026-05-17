using System.Diagnostics;

namespace TesteApp;

public static class TestRunner
{
    // Dispara a bateria de testes com coleta de cobertura de código integrada.
    public static async Task RunAllAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(">>> Iniciando bateria de testes com Code Coverage...");
        Console.ResetColor();

        try
        {
            var startInfo = GetProcessConfig();
            
            using var process = Process.Start(startInfo);
            if (process == null) 
            {
                throw new Exception("Falha ao subir o processo do 'dotnet test'. Verifique se o SDK está no PATH.");
            }

            using var reader = process.StandardOutput;
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                Console.WriteLine(line);
            }

            await process.WaitForExitAsync();

            HandleExit(process.ExitCode);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERRO] Falha crítica na execução: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
        }
    }

    // Adicionamos o coletor de cobertura 'XPlat Code Coverage' para gerar o relatório .xml
    private static ProcessStartInfo GetProcessConfig() => new()
    {
        FileName = "dotnet",
        Arguments = "test --logger:console --collect:\"XPlat Code Coverage\"",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        CreateNoWindow = true
    };

    private static void HandleExit(int exitCode)
    {
        Console.WriteLine();
        if (exitCode == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK] Bateria de testes concluída. Relatório de cobertura gerado em TestResults/");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[!] Finalizado com falhas. Alguns testes não passaram (Exit Code: {exitCode}).");
        }
        Console.WriteLine();
    }
}
