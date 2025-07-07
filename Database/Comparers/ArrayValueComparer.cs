using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MiataBot;

public sealed class ArrayValueComparer<T>() : ValueComparer<T[]>((l, r) => l!.SequenceEqual(r!, EqualityComparer<T>.Default), x => x.GetHashCode(), x => x.ToArray());