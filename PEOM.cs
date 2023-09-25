using System.Linq;
using System.Text.RegularExpressions;
namespace Lab2;


enum PEOM_LUT {
    A = 1000, //А - вихідне дане 1000+i - номер по графу;
    L_brace = 2000,// ( - відкриваюча скобка 2000;
    R_brace = 3000,// ) - закриваюча скобка 3000;
    OP = 4000,// ОP - операція алгоритму 4000+i - номер по графу;
    FO = 5000,// FO - результат функції 5000+i - номер по графу;

    /* //цього у нас в лабораторних задачах немає
    LF = 6000,//6000+ i-номер логічного оператора по графу - вихід логічного оператора зі значенням «1» Істина;
    LT = 7000,//7000+ i-номер логічного оператора по графу - вихід логічного оператора зі значенням «0» Брехня;
    */
}

class PEOM{
    public static string Graph2brace(Graph graph){
        string result = "(A1, A2, OP1),";//початок той самий, зхитруємо
        foreach(Node node in graph.Nodes.Skip(1)){
            result += "(";
            //заповнюємо FO, це входи з інших вершин
            //result += graph.Links.Where(link => link.Target == node.Id).Select(link => "FO" + link.Source).Aggregate((a, b) => a + ", " + b);
            IEnumerable<string> FOs = from link in graph.Links 
                                      where link.Target == node.Id 
                                      select "FO" + link.Source 
                                      into a select a;
            
            result += string.Join(", ", FOs.DefaultIfEmpty());
            if(node.Id == "1")
                result += "FO1, ";
            //заповнюємо OP, це вихід з нашої вершини
            IEnumerable<string> OPs = from link in graph.Links  
                                      where link.Source == node.Id 
                                      select "OP" + link.Target 
                                      into a select a;
            if(FOs.Count() > 0)
                result += ", ";

            result += "OP"+node.Id;//string.Join(", ", OPs.DefaultIfEmpty());
            result += "),";
        }
        result = result.Substring(0, result.Length - 1);
        return result;
    }
    private static readonly Regex matchbraces = new Regex(@"(?:\(([^\)]*)\),)");
    private static readonly Regex matchd = new Regex(@"\d+");
    public static string Machine(string formula){
        formula +=",";//regex fix
        string result = "";
        List<string> parts = matchbraces.Split(formula).Where(s => s != "").ToList();
        int[] ranks = new int[20];
        ranks[1] = 2;
        ranks[2] = 3;
        int i = 1;
        foreach(string part in parts){
            result += ((int)PEOM_LUT.L_brace) + " ";
            string[] ops = part.Split(", ");
            int rank = 1;
            foreach(string op in ops){
                int num = int.Parse(matchd.Match(op).Value);
                if(op.StartsWith("A"))
                    result += (PEOM_LUT.A + num) + " ";
                else if(op.StartsWith("OP"))
                    result += (PEOM_LUT.OP + num) + " ";
                else if(op.StartsWith("FO")){
                    result += (PEOM_LUT.FO + num) + " ";
                    rank = Math.Max(rank,ranks[num]);
                }
                else
                    Console.WriteLine("Unknown operation");
            }
            rank++;
            result += ((int)PEOM_LUT.R_brace) + "  |";
            result += " " + rank + " \n";  
            

            if(i>1)
                ranks[i] = rank;
            i++;
        }
        return result;
    }
}