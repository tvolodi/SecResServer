from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy import ForeignKey
from sqlalchemy.orm import relationship

from sqlalchemy import Column, Integer, String, Boolean, DateTime

Base = declarative_base()


class StmtType(Base):
    __tablename__ = 'StmtTypes'
    Id = Column(Integer, primary_key = True)
    Name = Column(String)


class SimFinSector(Base):
    __tablename__ = 'SimFinSectors'
    Id = Column(Integer, primary_key = True)
    Code = Column(Integer)
    Name = Column(String)


# Entity of SimFin portal
class SimFinEntity(Base):
    __tablename__ = 'SimFinEntities'
    Id = Column(Integer, primary_key = True)
    SimFinId = Column(Integer)
    Ticker = Column(String)
    Name = Column(String)
    IsStmtRegistryLoaded = Column(Boolean)
    LastUpdateDT = Column(DateTime)
    DeleteDT = Column(DateTime)


class SimFinIndustry(Base):
    __tablename__ = 'SimFinIndustries'
    Id = Column(Integer, primary_key = True)
    Code = Column(Integer)
    Name = Column(String)
    SimFinSectorId = Column(Integer, ForeignKey(SimFinSector.Id))
    SimFinSector = relationship("SimFinSector") # , foreign_keys = 'SimFinSector.Id')


class SimFinCompany(Base):
    __tablename__ = 'SimFinCompanies'
    Id = Column(Integer, primary_key = True)
    SimFinEntityId = Column(Integer, ForeignKey(SimFinEntity.Id))
    SimFinEntity = relationship("SimFinEntity")
    Name = Column(String)
    PYearEnd = Column(Integer)
    EmployeesQnt = Column(Integer)
    SimFinIndustryId = Column(Integer, ForeignKey(SimFinIndustry.Id))
    SimFinIndustry = relationship("SimFinIndustry")









