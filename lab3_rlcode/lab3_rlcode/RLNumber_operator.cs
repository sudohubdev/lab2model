using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
namespace Lab3;

public partial class RLNumber {
    #region operators
    public static RLNumber operator +(RLNumber a, RLNumber b) {
        if(a==0) return b;
        if(b==0) return a;
        RLNumber result = new("0.0");
        result.digits = a.digits.Concat(b.digits).ToList();
        result.Merge();
        return result;
    }
    public static RLNumber operator -(RLNumber aa, RLNumber bb) {
        //цей метод деструктивний, тому створюємо копію
        RLNumber? a = aa.Clone() as RLNumber;
        RLNumber? b = bb.Clone() as RLNumber;
        if(a is null || b is null) throw new Exception("Invalid RLNumber");
        
        //перевірка на 0
        // a - 0 = a
        if(b == 0) return a;
        if(a == 0){
            // 0 - a = -a
            b.sign = !b.sign;
            return b;
        }

        //перевірка -a--b = b-a
        if(a.sign && b.sign){
            a.sign = b.sign = false;
            (a, b) = (b, a);
        }
        //перевірка a--b= a+b
        if(!a.sign && b.sign){
            a.sign = b.sign = false;
            return a+b;
        }
        //перевірка -a-b = -(a+b)
        if(a.sign && !b.sign){
            a.sign = b.sign = false;
            RLNumber tmp = a+b;
            tmp.sign = true;
            return tmp;
        }
        

        //перевірка a>b інакше поміняти знак
        if(a < b){
            // a - b = -(b-a)
            RLNumber tmp = b - a;
            tmp.sign = !tmp.sign;
            return tmp;
        }

        while(b != 0){
            //крок 1: забрати дублікати ()
            a.digits.Intersect(b.digits).ToList().ForEach(x => {
                a.digits.Remove(x);
                b.digits.Remove(x);
            });
            Console.WriteLine("a: " + a.ToString());
            if(b == 0){
                if(a != 0)
                    a.Sort();
                return a;
            } 
            //крок 2: знайти найменше а яке більше ніж найбільше b
            int min = a.digits.Where(x => x > b.digits.Max()).Min();
            //крок 3: розщеплення найменшого b на 2 (b-1)
            a.digits.Remove(min);
            a.digits.Add(min-1);
            a.digits.Add(min-1);
        }
        a.Sort();
        return a;
    }
    public static RLNumber operator *(RLNumber a, RLNumber b) {
        RLNumber result = new RLNumber("0.0");
        if(a == 0 || b == 0) return result;

        result.sign = a.sign ^ b.sign;
        //Ni*Nk = Ni+Nk
        for (int i = 0; i < a.Count; i++)
        {
            for (int k = 0; k < b.Count; k++)
            {
                result.digits.Add(a.digits[i] + b.digits[k]);
            }
        }
        //зʼєднаємо однакові цифри
        result.Merge();
        return result;
    }

    public static bool operator <(RLNumber a, RLNumber b) {
        if(a == 0 && b == 0) return false;

        //compare digits
        for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
        {
            if(a.digits[i] < b.digits[i]) return true;
            if(a.digits[i] > b.digits[i]) return false;
        }

        return a.Count < b.Count;
    }
    public static bool operator >(RLNumber a, RLNumber b) {
        return !(a<b) && a!=b;
    } 
    #endregion
}