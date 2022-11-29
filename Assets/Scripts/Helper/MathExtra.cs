using Unity.Mathematics;

class MathExtra
{
    public static double SecondOrderEquationFirstPositive(double a, double b, double c)
    {
        // Check and solve for hidden first order equation
        // 0 = a*t^2 + b*t + c, if a=0 => 0 = b*t + c => t=-c/b
        // only care about the posititive case
        if (a == 0) return (-c / b) > 0 ? (-c / b) : double.MaxValue;

        // Check for only complex roots
        double D = b * b - 4 * a * c;
        if (D < 0) return double.MaxValue;

        // Solve SecondOrderEquation
        // https://en.wikipedia.org/wiki/Loss_of_significance
        double r1 = (-b - (b != 0 ? math.sign(b) : 1) * math.sqrt(D)) / (2 * a);
        double r2 = c / (a * r1);

        // Find the smallest strictly positive solution
        r1 = 0 < r1 ? r1 : double.MaxValue;
        r2 = 0 < r2 ? r2 : double.MaxValue;
        //double r = r1 < r2 ? r1 : r2;

        return r1 < r2 ? r1 : r2; // >= 0 ? r : double.MaxValue;
    }
}