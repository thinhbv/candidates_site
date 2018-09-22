namespace CMSSolutions.Web.UI.Navigation
{
    public interface INavigationProvider : IDependency
    {
        void GetNavigation(NavigationBuilder builder);
    }
}