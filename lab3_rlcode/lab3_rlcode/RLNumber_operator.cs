using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
namespace Lab3;

public partial class RLNumber {
    #region operators
    public static RLNumber operator +(RLNumber aa, RLNumber bb) {
        //цей метод деструктивний, тому створюємо копію
        RLNumber? a = aa.Clone() as RLNumber;
        RLNumber? b = bb.Clone() as RLNumber;
        if(a is null || b is null) throw new Exception("Invalid RLNumber");

        if(a==0) return b;
        if(b==0) return a;
        //перевіряємо знаки і шлемо на мінус
        // -a+b = b-a
        if(a.sign && !b.sign){
            a.sign = b.sign = false;
            return b-a;
        }
        // a+(-b) = a-b
        if(!a.sign && b.sign){
            a.sign = b.sign = false;
            return a-b;
        }
        RLNumber result = new("0.0");
        result.digits = a.digits.Concat(b.digits).ToList();
        result.Merge();

        // -a+(-b) = -(a+b)
        if(a.sign && b.sign)
            result.sign = true;
        
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
            //Console.WriteLine("a: " + a.ToString());
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

    public static RLNumber operator /(RLNumber aa, RLNumber bb) {
        RLNumber result = new("0.0");
        if(aa == 0) return result;
        if(bb == 0) {
            RLNumber inf = new("0.1.1024");//справжня... безкінечність?
            inf.sign = aa.sign ^ bb.sign;
            return inf;
        }

        //цей метод деструктивний, тому створюємо копію
        RLNumber? a = aa.Clone() as RLNumber ?? throw new Exception("Invalid RLNumber");
        RLNumber? b = bb.Clone() as RLNumber ?? throw new Exception("Invalid RLNumber");

        result.sign = a.sign ^ b.sign;
        /*
        1. перевірка на 0
        2. знаходження першої цифри знаходженням шляхом віднімання старшого
           розряду дільника зі старшим розрядом діленого
        3. множення цифри частки на дільник шляхом додавання до кожного розряду дільника
        4. порівняння діленогою якщо ділене >= добутку то виконуємо п5, інакше поточна
           цифра зменшужжться на одиниицю і переходимо до п2
        5. віднімання з діленого отриманого добутку
        6. перевірка критерію закінчення ділення відповідно до методу обробки. кщо виконується то
           переходимо до п7, інакше виконується пошук наступної цифри частки шляхом віднімання 
           старшого розряду дільника зі старшого залишку від ділення і перехід до п2
        7. знак
        8. кінець
        */
        
        //2. знаходження першої цифри знаходженням шляхом віднімання старшого
        int trap = 0;
        int firstDigit = 0;
        while(true){
            var differ = a.digits.Zip(b.digits, (x, y) => x - y);//різниця між цифрами
            
            try{
                firstDigit = differ.Where(i=>i!=0).First();//перша цифра яка не 0
            }
            catch(Exception){
                //якщо всі цифри 0 то виходимо
                Console.WriteLine("Всі цифри 0");
            }

            //result.digits.Add(firstDigit);//КОЛИ ЗАПИСУВАТИ В РЕЗУЛЬТАТ?
            firstDigit = Math.Abs(firstDigit);
            PTWO:
            //3. множення цифри частки на дільник шляхом додавання до кожного розряду дільника
            RLNumber? tmp = b.Clone() as RLNumber;
            if(tmp is null) throw new Exception("Invalid RLNumber");

            for (int i = 0; i < tmp.digits.Count; i++)
            {
                tmp.digits[i] += firstDigit;
            }
            trap++;
            if(a>=tmp){
                result.digits.Add(firstDigit);
                //5. віднімання з діленого отриманого добутку
                a -= tmp;
                //6. перевірка критерію закінчення ділення, результат віднімання = 0
                if(a == 0){
                    //зʼєднаємо однакові цифри
                    result.Merge();
                    result.sign = a.sign ^ b.sign;
                    return result;//0.8.-2.-4.-6.-8.-10.-12.-14.-16
                }
            }
            else{
                //інакше поточна цифра зменшується на одиниицю і переходимо до п2
                firstDigit--;
                goto PTWO;
            }
            //коли не ділиться націло зупиняємося через 10000 ітерацій або коли точність досягнута
            if(trap > 1000 || a < new RLNumber("0.1.-24")){
                return result;
            }

        }
    }

    public static bool operator <(RLNumber a, RLNumber b) {
        if(a == 0 && b == 0) return false;
        bool sign = a.sign ^ b.sign;
        //compare digits
        for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
        {
            if(a.digits[i] < b.digits[i]) return !sign;
            if(a.digits[i] > b.digits[i]) return sign;
        }

        return a.Count < b.Count;
    }
    public static bool operator >(RLNumber a, RLNumber b) {
        return !(a<b) && a!=b;
    } 
    public static bool operator >=(RLNumber a, RLNumber b) {
        return !(a<b) || a==b;
    } 
    public static bool operator <=(RLNumber a, RLNumber b) {
        return !(a>b) || a==b;
    } 
    #endregion
}