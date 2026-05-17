using System.Text;

namespace TesteApp.Reporting;

// Simples DTO para carregar os dados de cada teste.
// Usando record para aproveitar a imutabilidade e o código mais enxuto.
public record TestResult(
    string Name, 
    string Category, 
    bool Passed, 
    long DurationMs, 
    string? Error = null);

public class TestReportGenerator
{
    private readonly List<TestResult> _results = [];
    private readonly DateTime _createdAt;

    // Se não passar uma data, assume o "agora". Útil para mockar em testes unitários.
    public TestReportGenerator(DateTime? createdAt = null)
    {
        _createdAt = createdAt ?? DateTime.Now;
    }

    public void AddResult(TestResult result) => _results.Add(result);

    public string GenerateReport()
    {
        if (_results.Count == 0) return "Nenhum resultado registrado para gerar o relatório.";

        var sb = new StringBuilder();
        
        AppendHeader(sb);
        AppendSummary(sb);
        AppendCategories(sb);
        AppendFooter(sb);

        return sb.ToString();
    }

    // Desenha o topo do relatório com a data da execução.
    private void AppendHeader(StringBuilder sb)
    {
        sb.AppendLine(new string('═', 65));
        sb.AppendLine("                    TEST EXECUTION REPORT");
        sb.AppendLine(new string('═', 65));
        sb.AppendLine($"\nExecutado em: {_createdAt:dd/MM/yyyy HH:mm:ss}");
    }

    // Calcula as métricas gerais: taxa de sucesso, tempo total, etc.
    private void AppendSummary(StringBuilder sb)
    {
        var passed = _results.Count(r => r.Passed);
        var successRate = (double)passed / _results.Count * 100;
        var totalTime = _results.Sum(r => r.DurationMs);

        sb.AppendLine($"Total: {_results.Count} | Sucesso: {passed} | Falhas: {_results.Count - passed}");
        sb.AppendLine($"Taxa de Sucesso: {successRate:F1}% | Tempo Total: {totalTime}ms\n");
    }

    // Agrupa e lista os testes por categoria.
    private void AppendCategories(StringBuilder sb)
    {
        var groups = _results.GroupBy(r => r.Category);

        foreach (var group in groups)
        {
            sb.AppendLine("\n" + new string('─', 40));
            sb.AppendLine($"📦 {group.Key.ToUpper()}");
            sb.AppendLine(new string('─', 40));

            foreach (var test in group)
            {
                var icon = test.Passed ? "✓" : "×";
                sb.AppendLine($"  {icon} {test.Name} ({test.DurationMs}ms)");
                
                if (!test.Passed && !string.IsNullOrEmpty(test.Error))
                {
                    sb.AppendLine($"     ! Erro: {test.Error}");
                }
            }
        }
    }

    private void AppendFooter(StringBuilder sb)
    {
        sb.AppendLine($"\n{new string('═', 65)}");
    }

    public void SaveReport(string filePath)
    {
        File.WriteAllText(filePath, GenerateReport());
    }

    public void PrintReport()
    {
        Console.WriteLine(GenerateReport());
    }
}
