using System;
using System.Collections.Generic;
using System.Linq;
using Lab2;


string input = "V1V2↑2,5V3V4V5↑5,7V6↑6,11*V7V8V9↑9,13V10↓6V11V12↑12,15V13V14↑14,16V15V16V17";


if(System.Diagnostics.Debugger.IsAttached)
goto skip;//goto тки кращий за тищу вкладених ifів. Ви цього не бачили тсс

Console.WriteLine("Нагодуй мене рівнянням! або вкажи шлях до файлу з рівнянням");
if(args.Length>0){
    Console.WriteLine("Використовую файл " + args[0]);
    input = System.IO.File.ReadAllText(args[0]);
    //tee console output to file
    var ostrm = new FileStream ("./" + args[0] + "-solution.txt", FileMode.CreateNew, FileAccess.Write);
    var writer = new StreamWriter (ostrm);
    Console.SetOut(writer);
    writer.AutoFlush = true;
}
else{
    input = Console.ReadLine();
    if(input.Contains("↑")){
        Console.WriteLine("Зараз я його рішу!");
    }
    else{
        Console.WriteLine("Нема стрілочок. дивлюся такий файл");
        input = System.IO.File.ReadAllText(input);
    }
}
skip:
Console.WriteLine(input.ToIndexDigit());
Graph graph = Code2Graph.Calculate(input);
Console.WriteLine("Відкривайте студію і дивіться на граф!");

Console.WriteLine("2. Дужки ПЕОМ");
string formula = PEOM.Graph2brace(graph);
Console.WriteLine(formula.ToIndexDigit());
Console.WriteLine("3. Машинний код");
string machinecode = PEOM.Machine(formula);
Console.WriteLine(machinecode);
Console.WriteLine("4. Інтерпретатор");
Console.WriteLine("5. Аналіз рангів");