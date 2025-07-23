export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  hireDate: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  roles?: string[];
}

export interface UpdateUserProfileCommand {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
}

export interface UserProfile extends User {
  fullName: string;
  initials: string;
}
