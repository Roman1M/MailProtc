using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Windows;
using Microsoft.Win32;

namespace EmailApp
{
    public partial class MainWindow : Window
    {
        private string senderEmail;
        private string senderPassword;
        private SmtpClient smtpClient;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Логін користувача
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            senderEmail = UsernameTextBox.Text;
            senderPassword = PasswordBox.Password;

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
            {
                MessageBox.Show("Please enter your email and password.");
                return;
            }

            // Ініціалізація SMTP клієнта для підключення до сервера
            smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            MessageBox.Show("Login successful!");
            SendButton.IsEnabled = true;
        }

        // Відправка повідомлення
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string recipient = ToTextBox.Text;
            string subject = SubjectTextBox.Text;
            string body = BodyTextBox.Text;
            bool isImportant = ImportantCheckbox.IsChecked == true;

            if (string.IsNullOrEmpty(recipient) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            try
            {
                MailMessage mailMessage = new MailMessage(senderEmail, recipient, subject, body);
                if (isImportant)
                {
                    mailMessage.Priority = MailPriority.High;
                }

                // Додати вкладення
                if (AttachmentFiles != null)
                {
                    foreach (var filePath in AttachmentFiles)
                    {
                        mailMessage.Attachments.Add(new Attachment(filePath));
                    }
                }

                smtpClient.Send(mailMessage);
                MessageBox.Show("Email sent successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message);
            }
        }

        // Список вкладених файлів
        private List<string> AttachmentFiles = new List<string>();

        // Обробник кнопки для додавання вкладень
        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true; // Дозволяємо вибирати кілька файлів

            if (openFileDialog.ShowDialog() == true)
            {
                // Додаємо вибрані файли до списку вкладень
                AttachmentFiles.AddRange(openFileDialog.FileNames);
                MessageBox.Show("Files attached: " + string.Join(", ", AttachmentFiles));
            }
        }
    }
}
