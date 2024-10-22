public static class EmailTemplates
{
    static private (string Subject, string MailBody) _warnLowOfficerCount = (
        "[Announcement] Low Officer Count",
        "We wish to inform you that your shift {project} - {insert shift here} currently has {number} fewer officers signed up than anticipated. Please review and adjust your plans accordingly.\n\nBest regards,\n[Your Organization]"
    );
    static private (string Subject, string MailBody) _notClockedIn = (
        "[Announcement] Clock-In Reminder",
        "Dear [Recipient's Name],\n\nThis is a reminder that you have not yet clocked in for your shift {shift}. Please log in at your earliest convenience to add your clock-in time: {link}.\n\nThank you,\n[Your Organization]"
    );
    static public (string Subject, string MailBody) WarnLowOfficerCount => _warnLowOfficerCount;
    static public (string Subject, string MailBody) NotClockedIn => _notClockedIn;
}