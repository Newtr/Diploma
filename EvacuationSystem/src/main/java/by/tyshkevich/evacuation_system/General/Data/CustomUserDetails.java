package by.tyshkevich.evacuation_system.General.Data;

import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;

import java.util.Collection;
import java.util.Collections;

public class CustomUserDetails implements UserDetails
{

    private final User user;

    public CustomUserDetails(User user) {
        this.user = user;
    }

    @Override
    public Collection<? extends GrantedAuthority> getAuthorities()
    {
        if (user instanceof by.tyshkevich.evacuation_system.General.Data.Admin)
        {
            return Collections.singleton(new SimpleGrantedAuthority("ROLE_ADMIN"));
        }
        else
        {
            return Collections.singleton(new SimpleGrantedAuthority("ROLE_STUDENT"));
        }
    }

    @Override
    public String getPassword()
    {
        return null;
    }

    @Override
    public String getUsername()
    {
        if (user instanceof by.tyshkevich.evacuation_system.General.Data.Student)
        {
            return ((by.tyshkevich.evacuation_system.General.Data.Student) user).getStudentNumber();
        }
        else
        {
            return user.getLastName() + user.getFirstName();
        }
    }

    @Override
    public boolean isAccountNonExpired()
    {
        return true;
    }

    @Override
    public boolean isAccountNonLocked()
    {
        return true;
    }

    @Override
    public boolean isCredentialsNonExpired()
    {
        return true;
    }

    @Override
    public boolean isEnabled()
    {
        return true;
    }
}