using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityBot.Models
{
    public class UserSessionManager
    {
        private readonly ConcurrentDictionary<long, UserSession> _session = new ConcurrentDictionary<long, UserSession>();

        public UserSession GetSession(long userId)
        {
            if (!_session.TryGetValue(userId, out var session))
            {
                session = new UserSession();
                _session.TryAdd(userId, session);
            }
            return session;
        }
    }
}
