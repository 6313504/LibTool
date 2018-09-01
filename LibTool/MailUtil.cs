using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace DaiChong.Lib.Util
{
    public class MailUtil
    {
        /**/
        /// <summary>  
        /// 发送邮件  
        /// </summary>  
        public static void Send(string sendUser, string sendUserPwd, string sendSmtp, string toUser, List<string> ccUsers, string title, string context)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new System.Net.Mail.MailAddress(sendUser),
                Subject = title,
                Body = context,
                IsBodyHtml = true,
                BodyEncoding = System.Text.Encoding.UTF8,
                Priority = System.Net.Mail.MailPriority.Normal
            };
            mailMessage.To.Add(toUser);
            if (ccUsers != null)
            {
                foreach (string cc in ccUsers)
                {
                    if (!string.IsNullOrEmpty(cc))
                        mailMessage.CC.Add(cc);
                }
            }
            SmtpClient smtpClient = new SmtpClient
            {
                Credentials = new System.Net.NetworkCredential(mailMessage.From.Address, sendUserPwd),
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                Host = string.IsNullOrEmpty(sendSmtp) ? ("smtp." + mailMessage.From.Host) : sendSmtp

            };
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch
            {

            }
        }

        //带附件发送 
        public static bool SendMailAttach(string fromUser, string fromUserPwd, string fromUserName, string sendSmtp, string toUser, string toUserName, string cc, string subjectNm, string bodyAll, string fj)
        {
            bool ret = false;
            try
            {
                //Attachment objMailAttachment;
                //创建一个附件对象
                //objMailAttachment = new Attachment("d:\\test.txt");//发送邮件的附件

                Attachment objMailAttachment = new Attachment(fj);//发送邮件的附件

                MailMessage mm = new MailMessage();
                mm.From = new MailAddress(fromUser, fromUserName, Encoding.UTF8);
                mm.To.Add(toUser);
                string[] listc = null;
                try
                {
                    if (cc != null && cc != "")
                    {
                        listc = cc.Split(',');
                        for (int i = 0; i < listc.Length; i++)
                        {
                            mm.CC.Add(listc[i].ToString());
                        }

                    }
                }
                catch  
                {
                    throw  ;
                }

                mm.Attachments.Add(objMailAttachment);//将附件附加到邮件消息对象中


                mm.Subject = subjectNm;
                mm.SubjectEncoding = Encoding.UTF8;
                mm.Body = bodyAll;
                mm.BodyEncoding = Encoding.UTF8;
                mm.IsBodyHtml = true;
                //mm.Priority = MailPriority.High;//加急邮件!

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential(fromUser, fromUserPwd);
                client.Host = sendSmtp;
                client.Send(mm);
                ret = true;
            }
            catch 
            {
                throw  ;
            }
            return ret;
        }

    }
}
