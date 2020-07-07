using System.Threading.Tasks;

namespace RCMS_web.Services
{
    public interface IViewRenderer
    {
        Task<string> ViewRendererAsync<TModel>(string viewName, TModel model);
    }
}