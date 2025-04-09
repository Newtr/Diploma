package by.tyshkevich.evacuation_system.General.Data;

import org.springframework.data.jpa.repository.JpaRepository;
import java.util.Optional;


public interface AdminRepository extends JpaRepository<Admin, Long>
{
    Optional<Admin> findByLastName(String lastName);
}