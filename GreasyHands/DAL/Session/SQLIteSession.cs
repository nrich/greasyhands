using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace GreasyHands.DAL.Session
{
    public class MonoSqliteDriver : NHibernate.Driver.ReflectionBasedDriver
    {
        public MonoSqliteDriver()
            : base("Mono.Data.Sqlite", "Mono.Data.Sqlite.SqliteConnection", "Mono.Data.Sqlite.SqliteCommand")
        {

        }
        public override bool UseNamedPrefixInParameter
        {
            get
            {
                return true;
            }
        }

        public override bool UseNamedPrefixInSql
        {
            get
            {
                return true;
            }
        }
        public override string NamedPrefix
        {
            get
            {
                return "@";
            }
        }
        public override bool SupportsMultipleOpenReaders
        {
            get
            {
                return false;
            }
        }
    }

    public class SQLIteSession : ISession
    {
        public bool IsMono()
        {
            var platform = (int) Environment.OSVersion.Platform;
            return ((platform == 4) || (platform == 6) || (platform == 128));            
        }

        public ISessionFactory SessionFactory(string filename)
        {
            if (IsMono())
            {
                return
                    Fluently.Configure().Database(
                        SQLiteConfiguration.Standard.Driver<MonoSqliteDriver>().UsingFile("database.db")).Mappings(
                            m => m.FluentMappings.AddFromAssemblyOf<Container.Publisher>()).ExposeConfiguration(
                                cfg => new SchemaUpdate(cfg).Execute(false, true)).BuildSessionFactory();
            }

            return
                Fluently.Configure().Database(SQLiteConfiguration.Standard.UsingFile("database.db")).Mappings(
                    m => m.FluentMappings.AddFromAssemblyOf<Container.Publisher>()).ExposeConfiguration(
                        cfg => new SchemaUpdate(cfg).Execute(false, true)).BuildSessionFactory();
        }
    }
}
