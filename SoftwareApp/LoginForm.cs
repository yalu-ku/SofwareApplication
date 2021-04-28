using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using SoftwareApp.Model;
using System.Security.Cryptography;
using System.IO;

namespace SoftwareApp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public enum DesType //열거체
        {
            Encrypt = 0,    // 암호화
            Decrypt = 1     // 복호화
        }
        
        public class DES
        {
            // Key 값은 무조건 8자리여야한다.
            private byte[] Key { get; set; }
            // 암호화/복호화 메서드
            public string result(DesType type, string input)
            {
                var des = new DESCryptoServiceProvider()
                {
                    Key = Key,
                    IV = Key
                };
                var ms = new MemoryStream();
                // 익명 타입으로 transform / data 정의
                var property = new
                {
                    transform = type.Equals(DesType.Encrypt) ? des.CreateEncryptor() : des.CreateDecryptor(),
                    data = type.Equals(DesType.Encrypt) ? Encoding.UTF8.GetBytes(input.ToCharArray()) : Convert.FromBase64String(input)
                };
                var cryStream = new CryptoStream(ms, property.transform, CryptoStreamMode.Write);
                var data = property.data;

                cryStream.Write(data, 0, data.Length);
                cryStream.FlushFinalBlock();

                return type.Equals(DesType.Encrypt) ? Convert.ToBase64String(ms.ToArray()) : Encoding.UTF8.GetString(ms.GetBuffer());
            }

            // 생성자
            public DES(string key)
            {
                Key = ASCIIEncoding.ASCII.GetBytes(key);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tb_userid = textBox1.Text;
            string tb_userpw = textBox2.Text;
            int flag = 0;

            Console.WriteLine(tb_userid);
            Console.WriteLine(tb_userpw);
            using (var ctx = new ApplicationDataContext())
            {
                /*var item = ctx.login.SingleOrDefault(p => p.userid == "id06");
                item.userpasswd = textBox2.Text;*/

                // 항목의 개수로 로그인 가능 여부 확인
                /*var item = ctx.login.Where(a => a.userid == tb_userid && a.userpasswd == tb_userpw);
                Console.WriteLine("항목의 개수 : " + item.Count());
                flag = item.Count();*/

                var fransOrders = from ord in ctx.login //ord는 ctx.login 테이블에서 가져온 이름 
                                  where ord.userid == tb_userid
                                  select ord;
                var encrypt = "";
                foreach(var o in fransOrders)
                {
                    Console.WriteLine(o.userid + "\t" + o.userpasswd);
                    encrypt = o.userpasswd;
                }

                
                ctx.SaveChanges();

                var key = Properties.Resources.DecKey;
                var des = new DES(key);
                //복호화
                var decrypt = des.result(DesType.Decrypt, encrypt);
                Console.WriteLine(decrypt);

                //복호화한 값에서 널문자 제거
                string temp = decrypt.Replace('\0', ' ');
                temp = temp.Trim();

                if (temp == tb_userpw)
                {
                    flag = 1;
                }
                else
                {
                    flag = 0;
                }

            }

            if (flag == 0)
            {
                label3.Text = "로그인정보가 일치하지 않습니다.";
            }
            else if (flag == 1)
            {
                label3.Text = "로그인 되었습니다.";

                Hide();

                MainForm mf = new MainForm();
                mf.Show();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var ctx = new ApplicationDataContext())
            {
                var uid = textBox1.Text;
                var upw = textBox2.Text;

                // 암호화
                //var key = "test1234";
                var key = Properties.Resources.DecKey;
                var des = new DES(key);
                var encrypt = des.result(DesType.Encrypt, upw);

                //레코드 단위로 묶어주기
                var item = new tb_logininfo(); //generic type, 연동
                item.userid = uid;
                item.userpasswd = encrypt;

                ctx.login.Add(item);
                ctx.SaveChanges();
                /*context.Database.Log += (log)->
                {
                    Console.WriteLine($"log : {log}");
                };*/
                /*Console.WriteLine("데이터베이스 연결");
                var item = new tb_logininfo();
                *//*{
                    userid = "id123",
                    userpasswd = "pw123"
                };*/
                /*context.login.Add(loginInfo);
                context.SaveChanges();*//*
                item.userid = "id1234";
                item.userpasswd = "pw123";
                Console.WriteLine("레코드 정보 생성 완료");
                context.login.Add(item);
                Console.WriteLine("레코드 추가");
                context.SaveChanges();
                Console.WriteLine("커밋!");*/

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var ctx = new ApplicationDataContext())
            {
                var item = ctx.login.Where(p => p.dataid >= 1);

                foreach (var i in item)
                    ctx.login.Remove(i);

                ctx.SaveChanges();
            }

        }
    }


    public partial class ApplicationDataContext : DbContext
    {
        public ApplicationDataContext() : base("msgDBEntities")
        {
        }

        public DbSet<tb_logininfo> login { get; set; }

    }
}
