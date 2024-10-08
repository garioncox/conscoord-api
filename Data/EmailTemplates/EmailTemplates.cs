static class EmailTemplates {
static private (string Subject, string MailBody) _warnLowOfficerCount = (
    "[Announcement] Low Officer Count",
    "We wish to inform you that your shift {project} - {insert shift here} currently has {number} fewer officers signed up than anticipated. Please review and adjust your plans accordingly.\n\nBest regards,\n[Your Organization]"
);   
static private  (string Subject, string MailBody) _signUpShift = (
    "Shift Confirmation",
    "Dear [Recipient's Name],\n\nThank you for signing up for shift {insert shift here}. We look forward to seeing you at {times}. If you need to cancel or view your shift details, please click here: {shift}.\n\nWarm regards,\n[Your Organization]"
);
static private (string Subject, string MailBody) _officerCancellation = (
    "[Announcement] Shift Cancellation",
    "Dear [Recipient's Name],\n\nWe acknowledge that you have recently canceled your participation in the shift {shift}. If this cancellation was intentional, you can disregard this message. If it was not, please take the necessary steps to secure your account.\n\nSincerely,\n[Your Organization]"
);
static private (string Subject, string MailBody) _notClockedIn = (
    "[Announcement] Clock-In Reminder",
    "Dear [Recipient's Name],\n\nThis is a friendly reminder that you have not yet clocked in for your shift {shift}. Please log in at your earliest convenience to add your clock-in time: {link}.\n\nThank you,\n[Your Organization]"
);    
    static public (string Subject, string MailBody) SignUpShift => _signUpShift;
    static public (string Subject, string MailBody) WarnLowOfficerCount => _warnLowOfficerCount;
    static public (string Subject, string MailBody) OfficerCancellation => _officerCancellation;
    static public (string Subject, string MailBody) NotClockedIn => _notClockedIn;
}