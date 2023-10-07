using Lab3;
using System;

#region umova
/*
    Завдання 1
    Перевести номер залікової книжки/студентського квитка у двійкову систему
    числення та подати його у вигляді розрядно-логарифмічного числа.
    Завдання 2
    Перевести номер залікової книжки/студентського квитка у шістнадцяткову
    систему числення.
    Завдання 3
    Записати свій день (D) і місяць (М) народження (додатне та від’ємне подання D
    та М) у двійковій системі числення (8 бітів) та у вигляді РЛ-коду.
    Завдання 4
    Обчислити значення: A = 1 / (D + M) і B = (D + M) / (D * M) та записати їх у
    двійковій системі числення та у вигляді РЛ-коду.
    Завдання 5
    Знайти А (дробова частина) + В (дробова частина) та А (дробова частина) - В
    (дробова частина). Результати записати в шістнадцятковій системі числення,
    двійковій системі та РЛ-коді.
    Завдання 6

    Реалізувати переведення будь-якого дробового числа в десятковій формі у РЛ-
    код.

    2. Виконати письмово вправи No 1,2,6. (Лекція No4, останній слайд). Фото
    розмістити у звіті з лабораторної роботи до даного завдання.
    3. Підготувати звіт в електронному вигляді: титульна сторінка, тема, мета,
    постановка завдання, код програми, результати виконання.
*/
#endregion

int zalik = 123456789;
int D = 3;
int M = 1;

string zalikBinary = Convert.ToString(zalik, 2);
Console.WriteLine("1: Двійкове подання: " + zalikBinary);
Console.WriteLine("   Розрядно-логарифмічне подання: " + new RLNumber(zalik).ToString());

Console.WriteLine("2: Шістнадцяткове подання: " + Convert.ToString(zalik, 16));
Console.WriteLine("3: D₂ = " + Convert.ToString(D, 2));
Console.WriteLine("   M₂ = " + Convert.ToString(M, 2));
RLNumber RL_D = new(D);
RLNumber RL_M = new(M);
Console.WriteLine("   Dᵣₗ = " + RL_D.ToString());
Console.WriteLine("   Mᵣₗ = " + RL_M.ToString());

RLNumber RL_A = new(1.0d / (D + M));
RLNumber RL_B = new((double)(D + M) / (double)(D * M));

Console.WriteLine("4: A = " + RL_A.ToString());
Console.WriteLine("   B = " + RL_B.ToString());

RLNumber AB = RL_A + RL_B;
RLNumber AB_ = RL_A - RL_B;

Console.WriteLine("5: A + B = " + AB.ToString());
Console.WriteLine("   A - B = " + AB_.ToString());

Console.WriteLine(@"6: Інтерактивний РЛ калькулятор. 
вводьте числа у десятковому або РЛ поданні з операціями
у вигляді число оператор число.");
Console.WriteLine("Наприклад 123.456 - \"0.2.4.1\"");
while(true)
{
    try{
        Console.Write("\u001b[32mВведіть вираз>\u001b[0m ");
        string? input = Console.ReadLine();
        if (input == null || input.Trim().Length == 0) break;
        RLNumber result = Evaluator.EvaluateExpression(input);
        Console.WriteLine("Результат: "+ result.ToString() + " = " + result.ToDouble());
    }
    catch(Exception e){
        Console.WriteLine("Помилка: " + e.Message);
    }
}

