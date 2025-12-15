export interface UpdateUserRoles {
  roles: number[];
  nivelMembresia: number;
}

export enum RolEnum {
  OperadorAtraccion = 0,
  Visitante = 1,
  AdministradorParque = 2
}

export enum NivelMembresiaEnum {
  Estandar = 0,
  Premium = 1,
  VIP = 2
}

export const RoleStringToEnum: { [key: string]: number } = {
  'OperadorAtraccion': RolEnum.OperadorAtraccion,
  'Visitante': RolEnum.Visitante,
  'AdministradorParque': RolEnum.AdministradorParque
};

export const MembershipStringToEnum: { [key: string]: number } = {
  'Estándar': NivelMembresiaEnum.Estandar,
  'Premium': NivelMembresiaEnum.Premium,
  'VIP': NivelMembresiaEnum.VIP
};
