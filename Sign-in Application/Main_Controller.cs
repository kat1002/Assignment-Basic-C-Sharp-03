using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq.Expressions;

namespace Sign_in_Application
{
    internal class Main_Controller
    {

        #region Input
        private bool checkInput(string _option)
        {
            if (_option.Length > 2) return false;
            if (_option == "") return false;
            if (_option[0] < '1' || _option[0] > '3') return false;
            return true;
        }
        public void chooseOption(ref string _option)
        {
            _option = "";
            _option = Console.ReadLine();
            while (!checkInput(_option))
            {
                Console.WriteLine("Invalid option! Please choose again!");
                Console.Write("Choose your option: ");
                _option = Console.ReadLine();
            }

        }

        #endregion

        #region Read and Write Method
        public void WriteLine(string _path, string _line)
        {

            FileStream fs = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.None);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(_line);
            }
            
        }

        public bool checkExistingUsername(string _path, string _username)
        {

            FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.None);
            string _line, _Username;
            using (StreamReader sr = new StreamReader(fs))
            {
                while ((_line = sr.ReadLine()) != null)
                {
                    _Username = _line.Substring(0, _line.IndexOf(" "));
                    if (_username == _Username) return true;
                }
            }
            return false;
        }

        private bool signin(string _path, string _username, string _password)
        {
            FileStream fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            string _line, _Username, _Password;
            using (StreamReader sr = new StreamReader(fs))
            {
                while ((_line = sr.ReadLine()) != null)
                {
                    int pos = _line.IndexOf(" ");
                    _Username = _line.Substring(0, pos);
                    _Password = _line.Substring(pos + 1, _line.Length - pos - 1);
                    if (_username == _Username && _password == _Password) return true;
                }
            }
            return false;
        }
        #endregion

        #region Sign-up Option
        private bool checkUsername(string _username)
        {
           if (_username == "") return false;
           foreach(char _char in _username)
           {
                if ((_char >= '0' && _char <= '9') || (_char >= 'A' && _char <= 'Z') || (_char >= 'a' && _char <= 'z') || (_char == '_')) continue;
                return false;
           }

           return true;
        }

        private bool checkPassword(string _password)
        {
            if (_password == "") return false;
            if (_password.Contains(" ")) return false;
            return true;
        }

        private string enterPassword()
        {
        passwordEnter:
            var pass = string.Empty;

            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
            ConsoleKey key;

            do
            {
                keyInfo = Console.ReadKey(true);
                key = keyInfo.Key;

                if(key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass.Remove(pass.Length - 1, 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine();

            if(pass == "")
            {
                Console.Write("Invalid password! Please enter again: ");
                goto passwordEnter;
            }
            return pass;
        }

        private bool checkPassword(string _password, string _confirmPass)
        {
            return _password == _confirmPass;
        }
        
        public void sign_up(string _path)
        {
            string _username = "", _password = "", _confirmPass = "", _account = "";

        UsernameAssign:
            Console.Write("Enter your username: ");
            _username = Console.ReadLine();
            if (!checkUsername(_username))
            {
                Console.WriteLine("Invalid Character! Please try again!");
                goto UsernameAssign;
            }
            if (checkExistingUsername(_path, _username))
            {
                Console.WriteLine("This username has been taken!");
                goto UsernameAssign;
            }

        PasswordAssign:
            _password = "";
            _confirmPass = "";
            Console.Write("Enter your password: ");
            _password = enterPassword();
            
            Console.Write("Confirm your password: ");
            _confirmPass = enterPassword();

            if (!checkPassword(_password, _confirmPass))
            {
                Console.WriteLine("Password doesn't match! Please try again!");
                goto PasswordAssign;
            }
            
            _account = _username + " " + _password;

            WriteLine(_path, _account);

        chooseOption:
            string _option = "";
            Console.Write("Account created! Do you want to sign in? (Y/N) ");
            _option = Console.ReadLine();
            _option.Trim(); _option = _option.ToLower();
            if (_option == "y" || _option == "yes")
            {
                Console.Clear();
                sign_in(_path);
            }
            else if (_option == "n" || _option == "no") System.Environment.Exit(0);
            else
            {
                Console.Write("Invalid option! Please choose again: ");
                goto chooseOption;
            }
        }

        #endregion

        #region Sign-in Option
        public void sign_in(string _path)
        {
        StartSignin:
            string _username = "", _password = "";
            Console.Write("Enter your username: ");
            _username = Console.ReadLine();

            Console.Write("Enter your password: ");
            _password = enterPassword();

            if(signin(_path, _username, _password))
            {
                Console.WriteLine("You are signed in");
            }
            else
            {
                string _option = "";
                if(!checkExistingUsername(_path, _username))
                {
                    Console.Write("Doesnt find this username in the database! Do you want to sign up? (Y/N) ");
                    
                chooseOption:
                    _option = Console.ReadLine();
                    _option.Trim(); _option = _option.ToLower();
                    if (_option == "y" || _option == "yes")
                    {
                        Console.Clear();
                        sign_up(_path);
                    }
                    else if (_option == "n" || _option == "no") System.Environment.Exit(0);
                    else
                    {
                        Console.Write("Invalid option! Please choose again: ");
                        goto chooseOption;
                    } 
                }
                else
                {
                    Console.WriteLine("Wrong password!");
                    goto StartSignin;
                }
            }
        }
        #endregion
    }
}
