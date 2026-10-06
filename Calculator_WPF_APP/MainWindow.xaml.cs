using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Linq;

namespace Calculator_WPF_APP;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private LastInputType lastInputType = LastInputType.None;
    private string lastValue = "";
    
    private char[] operations = { '+', '-', '/', '*' };
    
    public MainWindow()
    {
        InitializeComponent();
    }
    
    public void AddNumber(object sender, RoutedEventArgs e)
    {
        if (sender is Button button &&
            int.TryParse(button.Content?.ToString(), out int digit))
        {
            lastInputType = LastInputType.Number;
            lastValue = digit.ToString();
            Display.Text = Display.Text == "0" ? Display.Text = digit.ToString() : Display.Text += digit.ToString();
        }
    }

    public void ClearDisplay(object sender, RoutedEventArgs e)
    {
        lastInputType = LastInputType.Number;
        lastValue = "0";
        Display.Text = "0";
    }

    public void AddOperation(object sender, RoutedEventArgs e)
    {
        Button button = (Button)sender;
        string op = button.Content.ToString();

        if (string.IsNullOrEmpty(Display.Text)) return;
        
        if (operations.Contains(Display.Text[^1]))
        {
            Display.Text = Display.Text[..^1] + op;
        }
        else
        {
            Display.Text += op;
        }
        lastInputType = LastInputType.Operation;
        lastValue = op;
    }

    public void Calculate(object sender, RoutedEventArgs e)
    {
        string thingToCalculate = Display.Text.ToString();

        try
        {
            DataTable table = new DataTable();
            var result = table.Compute(thingToCalculate, "");
            string resultText = result.ToString();

            if (resultText.Contains("Infinity") || resultText.Contains("inf") || resultText.Contains("∞") ||
                resultText.Contains("-∞") || resultText == "NaN")
            {
                Display.Text = "Error: Div by 0";
            }
            else
            {
                Display.Text = resultText;
            }

            lastValue = resultText[..^1];
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void ToggleNegativity(object sender, RoutedEventArgs e)
    {
        string display = Display.Text;
    
        if (string.IsNullOrEmpty(display) || display == "0") return;

        if (display.StartsWith("-") && !HasOperations(display[1..]))
        {
            Display.Text = display[1..];
        }
        else if (!HasOperations(display))
        {
            Display.Text = "-" + display;
        }
    }

    public void AddDot(object sender, RoutedEventArgs e)
    {
        string display = Display.Text;

        int lastOperationIndex = display.LastIndexOfAny(operations);
        string currentNumber = display[(lastOperationIndex + 1)..];

        if (currentNumber.Contains("."))
            return;

        if (lastOperationIndex == display.Length - 1)
        {
            Display.Text += "0.";
        }
        else
        {
            Display.Text += ".";
        }

        lastInputType = LastInputType.DecimalPoint;
        lastValue = ".";
    }

    private bool HasOperations(string text)
    {
        return text.Any(c => operations.Contains(c));
    }
}