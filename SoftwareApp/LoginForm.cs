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

namespace SoftwareApp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
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
                var item = ctx.login.Where(a => a.userid == tb_userid && a.userpasswd == tb_userpw);
                Console.WriteLine("항목의 개수 : " + item.Count());
                flag = item.Count();

                ctx.SaveChanges();
            }

            if(flag == 0)
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
        private void button2_Click(object sender, EventArgs e) //정보추가
        {
            using (var ctx = new ApplicationDataContext())
            {
                var uid = textBox1.Text;
                var upw = textBox2.Text;

                var item = new tb_logininfo(); //generic type 연동
                item.userid = uid;
                ctx.login.Add(항목);
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

                foreach(var i in item)
                    ctx.login.Remove(i);

                ctx.SaveChanges();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public partial class ApplicationDataContext : DbContext
        {
            public ApplicationDataContext() : base("msgDBEntities")
            {
            }

            public DbSet<tb_logininfo> login { get; set; }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
