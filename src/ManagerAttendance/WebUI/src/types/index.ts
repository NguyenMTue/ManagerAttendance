export type RoleType = 'Admin' | 'Manager' | 'Employee';

export type AttendanceStatus = 'Present' | 'Late' | 'Absent';

export type BandType = 'Intern' | 'Junior' | 'Mid' | 'Senior' | 'Lead' | 'Principal';

export type DepartmentType = 'Development' | 'QA' | 'Management' | 'HR' | 'IT' | 'Finance' | 'Sales' | 'Marketing';

export type GenderType = 'Male' | 'Female' | 'Other';

export type ManagerType = 'Technical' | 'Project' | 'Operations' | 'General';

export interface User {
  id: string;
  email: string;
  role: RoleType;
  employeeId?: number;
}

export interface AuthResponseDto {
  token: string;
  email: string;
  role: string;
  expiration: string;
  userId: string;
  employeeId?: number;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  role?: string;
}

export interface AttendanceRecordDto {
  id: number;
  employeeId: number;
  employeeName: string;
  employeeEmail: string;
  arrivalTime: string;
  departureTime?: string;
  status: AttendanceStatus | string;
  notes?: string;
  createdAt: string;
}

export interface CheckInDto {
  employeeId?: number;
  notes?: string;
}

export interface CheckOutDto {
  employeeId?: number;
  notes?: string;
}

export interface EmployeeDto {
  id: number;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  gender: GenderType | string;
  department: DepartmentType | string;
  band: BandType | string;
  isActive: boolean;
  employeeType: string;
  technicalDirection?: string;
  codingSkillsFlag?: string;
  testingMethodology?: string;
  automationSkills?: boolean;
  managerType?: ManagerType | string;
  managedDepartment?: string;
}

export interface CreateDeveloperDto {
  userId?: string;
  firstName: string;
  lastName: string;
  email: string;
  gender?: GenderType;
  department?: DepartmentType;
  band?: BandType;
  technicalDirection: string;
  codingSkillsFlag: string;
}

export interface CreateQADto {
  userId?: string;
  firstName: string;
  lastName: string;
  email: string;
  gender?: GenderType;
  department?: DepartmentType;
  band?: BandType;
  testingMethodology: string;
  automationSkills: boolean;
}

export interface CreateManagerDto {
  userId?: string;
  firstName: string;
  lastName: string;
  email: string;
  gender?: GenderType;
  department?: DepartmentType;
  band?: BandType;
  managerType: ManagerType;
  managedDepartment: string;
}

export interface PromoteEmployeeDto {
  band: BandType;
  department?: DepartmentType;
}

export interface UpdateEmployeeStatusDto {
  isActive: boolean;
  reason?: string;
}

export interface ExcelRowErrorDto {
  rowIndex: number;
  fieldName: string;
  errorMessage: string;
  rawData?: string;
}

export interface ExcelImportResultDto {
  isDryRun: boolean;
  totalRows: number;
  successCount: number;
  errorCount: number;
  importedEmployees: EmployeeDto[];
  errors: ExcelRowErrorDto[];
}
