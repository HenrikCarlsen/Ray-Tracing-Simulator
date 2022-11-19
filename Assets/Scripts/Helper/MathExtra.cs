using Unity.Mathematics;

class MathExtra
{
    public static double SecondOrderEquationFirstPositive(double a, double b, double c)
    {
        // First order equation
        if (a == 0) return 0 < b ? (-c / b) : double.MaxValue;

        // Only complex roots
        double D = b * b - 4 * a * c;
        if (D < 0) return double.MaxValue;

        // https://en.wikipedia.org/wiki/Loss_of_significance
        double r1 = (-b - (b != 0 ? math.sign(b) : 1) * math.sqrt(D)) / (2 * a);
        double r2 = c / (a * r1);

        r1 = 0 <= r1 ? r1 : double.MaxValue;
        r2 = 0 <= r2 ? r2 : double.MaxValue;

        double r = r1 < r2 ? r1 : r2;

        return r >= 0 ? r : double.MaxValue;
    }
}