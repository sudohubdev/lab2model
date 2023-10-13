using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab3;

namespace Prime.UnitTests.Services
{

    class TestData
    {
        public string a;
        public string b;
        public string result;
        public TestData(string num1, string num2, string result)
        {
            this.a = num1;
            this.b = num2;
            this.result = result;
        }
    }

    [TestClass]
    public class TestMinus
    {   
        List<TestData> dataMinus = new List<TestData>(){
            //приклади з книжки
            new TestData("0.7.11.9.4.0.-2.-7.-9","0.5.8.6.4.-1.-2",  "0.6.11.7.6.-1.-7.-9"),
            new TestData(
                "0.10.45.38.23.22.18.11.8.7.4.0",
                "0.12.45.37.34.33.22.20.19.16.14.12.11.10",  
                "0.15.36.35.33.22.21.19.17.15.13.11.10.8.7.4.0"
            ),
        };
        
        [TestMethod]
        public void TestMinus1() {
            foreach(TestData equation in dataMinus){
                RLNumber num = new(equation.a);
                RLNumber num2 = new(equation.b);
                RLNumber result = num - num2;
            
                RLNumber expected = new(equation.result);
                Console.WriteLine("Результат РЛ віднімання: " + result.ToString());
                Console.WriteLine("Результат РЛ в десятковій системі: " + result.ToDouble());

                Assert.AreEqual(expected.ToString(), result.ToString());
                Assert.AreEqual(expected.ToDouble(), result.ToDouble());
            }
        }
        [TestMethod]
        public void TestPlusRNG(){
            //init using current time
            Random rand = new((int)DateTime.Now.Ticks);
            Parallel.For(0, 10000, i =>
            {
                RLNumber num = new(rand.NextDouble()*1000 - 500);
                RLNumber num2 = new(rand.NextDouble()*1000 - 500);
                RLNumber result = num + num2;
                double expected = num.ToDouble() + num2.ToDouble();
                Console.WriteLine("Результат РЛ додавання: " + result.ToString());
                Console.WriteLine("Результат РЛ в десятковій системі: " + result.ToDouble());

                double diff = Math.Abs(expected - result.ToDouble());
                Assert.IsTrue(diff < 0.0001);
            });
        }
        [TestMethod]
        public void TestMinusRNG(){
            Random rand = new((int)DateTime.Now.Ticks);
            Parallel.For(0, 10000, i =>
            {
                RLNumber num = new(rand.NextDouble()*1000-500);
                RLNumber num2 = new(rand.NextDouble()*1000-500);
                RLNumber result = num - num2;
                double expected = num.ToDouble() - num2.ToDouble();
                Console.WriteLine("Результат РЛ віднімання: " + result.ToString());
                Console.WriteLine("Результат РЛ в десятковій системі: " + result.ToDouble());

                double diff = Math.Abs(expected - result.ToDouble());
                Assert.IsTrue(diff < 0.0001);
            });
        }
        [TestMethod]
        public void TestMulRNG(){
            Random rand = new((int)DateTime.Now.Ticks);
            Parallel.For(0, 10000, i =>
            {
                RLNumber num = new(rand.NextDouble()*1000-500);
                RLNumber num2 = new(rand.NextDouble()*1000-500);
                RLNumber result = num * num2;
                double expected = num.ToDouble() * num2.ToDouble();
                Console.WriteLine("Результат РЛ множення: " + result.ToString());
                Console.WriteLine("Результат РЛ в десятковій системі: " + result.ToDouble());

                double diff = Math.Abs(expected - result.ToDouble());
                Assert.IsTrue(diff < 0.0001);
            });
        }
        [TestMethod]
        public void TestDivRNG(){
            Random rand = new((int)DateTime.Now.Ticks);
            for(int i = 0; i < 100; i++)
            //Parallel.For(0, 100000, i =>
            {
                RLNumber num = new(rand.NextDouble()*1000-500);
                RLNumber num2 = new(rand.NextDouble()*1000-500);
                if(num2 == 0) num2 = new(1);

                RLNumber result = num / num2;
                double expected = num.ToDouble() / num2.ToDouble();
                Console.WriteLine("Результат РЛ ділення: " + result.ToString());
                Console.WriteLine("Результат РЛ в десятковій системі: " + result.ToDouble());

                double diff = Math.Abs(expected - result.ToDouble());
                Assert.IsTrue(diff < 0.0001);
            //});
            }
        }


        [TestMethod]
        public void TestMultiply1() {
            RLNumber num = new("0.2.2.0");//5
            RLNumber num2 = new("0.3.2.1.0");//7
            RLNumber result = num * num2;
            RLNumber expected = new(35);
            Console.WriteLine("Результат РЛ множення: " + result.ToString());
            
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [TestMethod]
        public void AcceptanceTest() {
            RLNumber num = new(-5);
            RLNumber num2 = new(405.1005859375);
            RLNumber result = num - num2;
            double expected = -410.1005859375;
            Console.WriteLine("Результат РЛ" + result);

            double diff = Math.Abs(expected - result.ToDouble());
            Assert.IsTrue(diff < 0.0001,$"Expected {expected}, got {result.ToDouble()}");
        }
    }
}