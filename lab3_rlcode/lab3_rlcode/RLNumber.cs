using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Lab3;

[System.Diagnostics.DebuggerDisplay("{ToString()} ({ToDouble()})")]
public partial class RLNumber:ICloneable {
    List<int> digits;
    bool sign = false;
    public RLNumber(string number, bool isPrefixed = true) {
        digits = new List<int>();
        List<string> groups = Regex.Split(number, @"(-*\d+)").ToList();
        groups.RemoveAll(x => x == "");
        groups.RemoveAll(x => x == ".");
        digits = groups.Select(x => int.Parse(x)).ToList();
        if(isPrefixed) {
            //зняли знак
            sign = digits[0] == 1;
            digits.RemoveAt(0);
            //зняли кількість цифр
            int count = digits[0];
            digits.RemoveAt(0);
            if(count != digits.Count) {
                throw new Exception("Invalid RLNumber");
            }
        }
    }
    public RLNumber(bool sign, List<int> digits){
        this.sign = sign;
        this.digits = digits;
    }
    public RLNumber(double number, int precision=10, int maxexp = -16){
        if(number == 0){
            digits = new List<int>();
            return;
        }
        sign = number<0;
        number = Math.Abs(number);
        //create a list with powers of twos
        List<int> powersOf2 = new List<int>();
        double remaining = number;
        
        for (int i = 0; i < precision; i++)
        {
            int exponent = (int)Math.Floor(Math.Log2(remaining));
            double power = Math.Pow(2, exponent);

            if (power > remaining)
                exponent--;

            powersOf2.Add(exponent);
            remaining -= Math.Pow(2, exponent);
            if(Math.Abs(remaining) < Math.Pow(2,maxexp)){
                break;
            }
        }
        digits = powersOf2;
        Merge();
    }   
    public int Count {
        get {
            return digits.Count;
        }
    }
    public void Sort(){
        //reverse sort
        digits.Sort((a,b)=>b-a);
    }
    public void Merge(){
        Sort();
        //merge same digits to i+1
        bool done=false;
        while(!done){//деклька ітерацій поки у нас вже не буде ніяких пар
            done=true;
            for (int i = 0; i < Count-1; i++)
            {
                if(digits[i] == digits[i+1]){
                    digits[i+1]++;
                    digits.RemoveAt(i);
                    done=false;
                }

            }
        }
    }

    // конвертери в десяткову і в строку
    public override string ToString() {
        return (sign ? "1" : "0") + "." + Count + (digits.Count>0?".":"") + //знак і кількість цифр
        string.Join(".", digits.Select(d => d.ToString()));//РЛ-код
    }
    public double ToDouble(){
        if(digits.Count == 0) return 0;
        //add 2^number 
        return digits.Select(num=>Math.Pow(2,num)).Aggregate((a,b)=>a+b) * (sign ? -1 : 1);
    }


    public override int GetHashCode()
    {
        return digits.GetHashCode();
    }
    public override bool Equals(object? obj)
    {   
        if(obj == null) return false;
        return this == (RLNumber)obj;
    }
    #region compare
    public static bool operator ==(RLNumber a, RLNumber b) {
        if(a.sign != b.sign) return false;
        if(a.Count != b.Count) return false;
        //якщо хоча б одна цифра не співпадає, то вони не рівні
        for (int i = 0; i < a.Count; i++)
        {
            if(a.digits[i] != b.digits[i]) return false;
        }
        return true;
    }
    public static bool operator !=(RLNumber a, RLNumber b) {
        return !(a==b);
    }
    public static bool operator ==(RLNumber a, double b) {
        return a.ToDouble() == b;
    }
    public static bool operator !=(RLNumber a, double b) {
        return !(a==b);
    }
    #endregion
    public object Clone()
    {
        return new RLNumber(sign, digits.ToList());   
    }
}