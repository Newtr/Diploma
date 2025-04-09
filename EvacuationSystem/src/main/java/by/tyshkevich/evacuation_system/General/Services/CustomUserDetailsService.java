package by.tyshkevich.evacuation_system.General.Services;

import by.tyshkevich.evacuation_system.General.Data.CustomUserDetails;
import by.tyshkevich.evacuation_system.General.Data.StudentRepository;
import by.tyshkevich.evacuation_system.General.Data.Student;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;

@Service
public class CustomUserDetailsService implements UserDetailsService
{

    @Autowired
    private StudentRepository studentRepository;

    @Override
    public UserDetails loadUserByUsername(String username) throws UsernameNotFoundException
    {
        Student student = studentRepository.findByStudentNumber(username)
                .orElseThrow(() -> new UsernameNotFoundException("Студент с номером " + username + " не найден"));
        return new CustomUserDetails(student);
    }
}