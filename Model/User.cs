using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class User
    {
        //一创建IsLoggined就是false，所有Field和Property全部都赋默认值
        public User()
        {
            IsLoggined = false;
            Id = "root";
            password = "123456";
        }
        public static User SetUser(string id,string pwd)
        {
            User user = new User();
            user.Id = id;
            user.password = pwd;
            user.IsLoggined = true;
            return user;
        }
        public string Id { get; set; }
        public bool IsLoggined { get; private set; }

        public string password;
    }
}
