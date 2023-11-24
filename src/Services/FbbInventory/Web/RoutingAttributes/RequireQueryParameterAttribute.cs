using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Bazaar.FbbInventory.Web.RoutingAttributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RequireQueryParameterAttribute : Attribute, IActionConstraint
{
    private readonly string _parameterName;
    public int Order { get; }

    public RequireQueryParameterAttribute(string parameterName)
    {
        _parameterName = parameterName;
    }

    public bool Accept(ActionConstraintContext context)
    {
        return context.RouteContext.HttpContext.Request.Query.Keys.Contains(_parameterName);
    }
}
