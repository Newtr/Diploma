package by.tyshkevich.evacuation_system.General.Data;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface StudentRepository extends JpaRepository<Student, Long>
{
    Optional<Student> findByStudentNumber(String studentNumber);
    Optional<Student> findByTelegramChatId(String telegramChatId);
}
