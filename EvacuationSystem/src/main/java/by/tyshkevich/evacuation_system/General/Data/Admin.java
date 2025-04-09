package by.tyshkevich.evacuation_system.General.Data;

import jakarta.persistence.*;
import java.time.LocalDate;

@Entity
@Table(name = "admins")
public class Admin extends User
{

    private String department;
    private String academicDegree;

    private LocalDate privilegeGranted;
    private LocalDate privilegeExpiry;

    public String getDepartment()
    {
        return department;
    }

    public void setDepartment(String department)
    {
        this.department = department;
    }

    public String getAcademicDegree()
    {
        return academicDegree;
    }

    public void setAcademicDegree(String academicDegree)
    {
        this.academicDegree = academicDegree;
    }

    public LocalDate getPrivilegeGranted()
    {
        return privilegeGranted;
    }

    public void setPrivilegeGranted(LocalDate privilegeGranted)
    {
        this.privilegeGranted = privilegeGranted;
    }

    public LocalDate getPrivilegeExpiry()
    {
        return privilegeExpiry;
    }

    public void setPrivilegeExpiry(LocalDate privilegeExpiry)
    {
        this.privilegeExpiry = privilegeExpiry;
    }
}
