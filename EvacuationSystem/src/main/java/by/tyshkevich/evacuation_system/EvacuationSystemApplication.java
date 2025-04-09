package by.tyshkevich.evacuation_system;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;

@SpringBootApplication
public class EvacuationSystemApplication
{

	public static void main(String[] args)
	{
		SpringApplication.run(EvacuationSystemApplication.class, args);
		System.out.println("System Online!");
	}

}
