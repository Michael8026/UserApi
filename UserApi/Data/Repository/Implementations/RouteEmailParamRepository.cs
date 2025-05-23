using api.Data;
using api.Interfaces;
using api.Models;

namespace api.Repository;
public partial class RouteEmailParamRepository : IRouteEmailParamRepository
{

    private readonly ApplicationDbContext _context;

    public RouteEmailParamRepository(ApplicationDbContext context)
    {
        _context = context;
    }



    public RouteEmailParam GetRouteEmailParams()
    {
        return _context.RouteEmailParams.FirstOrDefault();
    }



}
