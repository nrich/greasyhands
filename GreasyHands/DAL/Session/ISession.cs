using NHibernate;

namespace GreasyHands.DAL.Session
{
    public interface ISession
    {
        ISessionFactory SessionFactory(string filename);
    }
}
