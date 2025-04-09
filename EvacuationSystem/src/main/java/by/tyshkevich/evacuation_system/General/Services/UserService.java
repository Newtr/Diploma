package by.tyshkevich.evacuation_system.General.Services;

import by.tyshkevich.evacuation_system.General.Data.Student;
import by.tyshkevich.evacuation_system.General.Data.StudentRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.Optional;

@Service
public class UserService
{

    @Autowired
    private StudentRepository studentRepository;

    public boolean isUserLoggedIn(long chatId)
    {
        Optional<Student> student = studentRepository.findByTelegramChatId(String.valueOf(chatId));
        return student.isPresent();
    }

    public String loginStudent(String studentNumber, String telegramChatId)
    {
        Optional<Student> studentOpt = studentRepository.findByStudentNumber(studentNumber);
        if (studentOpt.isPresent()) {
            Student student = studentOpt.get();
            student.setTelegramChatId(telegramChatId);
            studentRepository.save(student);
            return "Добро пожаловать, студент " + student.getLastName() + "!";
        }
        else
        {
            return "Студент с номером " + studentNumber + " не найден.";
        }
    }
}
