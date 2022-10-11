namespace Vortex.GenerativeArtSuite.Common.ViewModels
{
    public interface IViewModel<TModel>
    {
        TModel Model { get; }
    }
}
