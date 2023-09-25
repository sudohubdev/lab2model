
//using Microsoft.Visualstudio.GraphModel;
using System.Text.RegularExpressions;
namespace Lab2;
public class Code2Graph
{
    private static Regex regex = new Regex(@"\d+");
    private static Regex searchstar = new Regex(@"^.([^V]*)\*");
    public static Graph Calculate(string equation)
    {
        string temp = equation;
        Graph graph = new Graph();
        while(temp.Length>0){
            char c = temp[0];

            if(c=='V'){
                string parseint = regex.Match(temp).Value;
                int realint = Int32.Parse(parseint);
                graph.Nodes.Add(new Node(parseint, "OP"+parseint));
                temp = temp.Substring(parseint.Length + 1);
                //node to the next V
                if(realint<17 && !searchstar.IsMatch(temp)){
                    graph.Links.Add(new Link(parseint, (realint+1).ToString(), "FO" + parseint));
                }
            }
            else if(c=='↑'){
                string from = regex.Match(temp).Value;
                temp = temp.Substring(from.Length + 2);
                string to = regex.Match(temp).Value;

                graph.Links.Add(new Link(from, to, "FO"+ from));
                temp = temp.Substring(to.Length);
            }
            else{
                temp = temp.Substring(1);
                //throw new Exception("Invalid input");
            }
            /*else if(c=='↓'){
                graph.Links.Add(new Link(temp[i-1].ToString(), temp[i+1].ToString(), "Dash"));
                i+=2;
            }*/
        }
        //print out the graph
        DGMLWriter.Serialize(graph, "graph.dgml");
        return graph;
    }
}
