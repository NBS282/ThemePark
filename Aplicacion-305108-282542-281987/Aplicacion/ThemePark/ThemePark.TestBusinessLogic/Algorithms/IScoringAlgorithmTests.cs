using ThemePark.Entities;
using ThemePark.IBusinessLogic;

namespace ThemePark.TestBusinessLogic.Algorithms;

[TestClass]
public class ScoringAlgorithmTests
{
    [TestMethod]
    public void IScoringAlgorithm_ShouldHaveCalculatePointsMethodWithConfiguracion()
    {
        var algorithmType = typeof(IScoringAlgorithm);
        var calculatePointsMethod = algorithmType.GetMethod("CalculatePoints", [typeof(Visit), typeof(Configuracion), typeof(List<Visit>)]);
        Assert.IsNotNull(calculatePointsMethod, "IScoringAlgorithm should have CalculatePoints method with Configuracion");
        Assert.AreEqual(typeof(int), calculatePointsMethod.ReturnType, "CalculatePoints should return int");
    }

    [TestMethod]
    public void IScoringAlgorithm_CalculatePoints_ShouldHaveCorrectParameterCount()
    {
        var algorithmType = typeof(IScoringAlgorithm);
        var calculatePointsMethod = algorithmType.GetMethod("CalculatePoints", [typeof(Visit), typeof(Configuracion), typeof(List<Visit>)]);
        var parameters = calculatePointsMethod!.GetParameters();
        Assert.AreEqual(3, parameters.Length, "CalculatePoints should have 3 parameters");
    }

    [TestMethod]
    public void IScoringAlgorithm_CalculatePoints_ShouldHaveCorrectParameterTypes()
    {
        var algorithmType = typeof(IScoringAlgorithm);
        var calculatePointsMethod = algorithmType.GetMethod("CalculatePoints", [typeof(Visit), typeof(Configuracion), typeof(List<Visit>)]);
        var parameters = calculatePointsMethod!.GetParameters();
        Assert.AreEqual(typeof(Visit), parameters[0].ParameterType, "First parameter should be Visit");
        Assert.AreEqual(typeof(Configuracion), parameters[1].ParameterType, "Second parameter should be Configuracion");
        Assert.AreEqual(typeof(List<Visit>), parameters[2].ParameterType, "Third parameter should be List<Visit>");
    }
}
