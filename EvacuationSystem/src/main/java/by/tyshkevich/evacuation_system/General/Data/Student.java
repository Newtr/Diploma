package by.tyshkevich.evacuation_system.General.Data;

import jakarta.persistence.*;
import java.time.LocalDate;

@Entity
@Table(name = "students")
public class Student extends User
{

    @Column(length = 8, unique = true, nullable = false)
    private String studentNumber;

    private String faculty;
    private String formOfStudy;

    private LocalDate admissionDate;
    private LocalDate ticketIssueDate;
    private LocalDate ticketExpiryDate;

    @Column(unique = true)
    private String telegramChatId;

    public String getStudentNumber()
    {
        return studentNumber;
    }

    public void setStudentNumber(String studentNumber)
    {
        this.studentNumber = studentNumber;
    }

    public String getFaculty()
    {
        return faculty;
    }

    public void setFaculty(String faculty)
    {
        this.faculty = faculty;
    }

    public String getFormOfStudy()
    {
        return formOfStudy;
    }

    public void setFormOfStudy(String formOfStudy)
    {
        this.formOfStudy = formOfStudy;
    }

    public LocalDate getAdmissionDate()
    {
        return admissionDate;
    }

    public void setAdmissionDate(LocalDate admissionDate)
    {
        this.admissionDate = admissionDate;
    }

    public LocalDate getTicketIssueDate()
    {
        return ticketIssueDate;
    }

    public void setTicketIssueDate(LocalDate ticketIssueDate)
    {
        this.ticketIssueDate = ticketIssueDate;
    }

    public LocalDate getTicketExpiryDate()
    {
        return ticketExpiryDate;
    }

    public void setTicketExpiryDate(LocalDate ticketExpiryDate)
    {
        this.ticketExpiryDate = ticketExpiryDate;
    }

    public String getTelegramChatId() {
        return telegramChatId;
    }

    public void setTelegramChatId(String telegramChatId) {
        this.telegramChatId = telegramChatId;
    }
}